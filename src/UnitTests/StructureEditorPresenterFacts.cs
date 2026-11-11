// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Linq;
using AwesomeAssertions;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.NewtonsoftJson;
using NanoByte.StructureEditor.Sample;
using NanoByte.StructureEditor.Sample.Model;
using Xunit;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor;

public class StructureEditorPresenterFacts
{
    private sealed class FakeEditor<T> : INodeEditor<T> where T : class
    {
        public T? Target { get; set; }
        public ICommandExecutor? CommandExecutor { get; set; }
        public void Refresh() {}
    }

    private readonly AddressBook _addressBook = SampleData.AddressBook;
    private readonly StructureEditorPresenter<AddressBook> _presenter = new(() => new AddressBook());
    private int _structureChangedCount;

    public StructureEditorPresenterFacts()
    {
        _presenter.DescribeRoot<FakeEditor<AddressBook>>("Address Book")
                  .AddList("Group", x => x.Groups, () => new Group(), new FakeEditor<Group>());
        _presenter.Describe<IContactContainer>()
                  .AddList("Contact", x => x.Contacts, () => new Contact(), new FakeEditor<Contact>());

        _presenter.StructureChanged += () => _structureChangedCount++;
        _presenter.Open(CommandManager.For(_addressBook));
    }

    private Node Root => _presenter.GetRootNodes().Single();

    private void Select(Node? node) => _presenter.Select(node, out _, out _);

    [Fact]
    public void BuildsTree()
    {
        Root.NodeType.Should().Be("Address Book");
        Root.Target.Should().BeSameAs(_addressBook);

        _presenter.GetNodes(Root.Target).Select(x => (x.NodeType, x.Target)).Should().Equal(
            ("Group", _addressBook.Groups[0]),
            ("Contact", _addressBook.Contacts[0]));

        _presenter.GetNodes(_addressBook.Groups[0]).Select(x => x.Target).Should().Equal(_addressBook.Groups[0].Contacts);
        _presenter.GetNodes(null).Should().BeEmpty();
    }

    [Fact]
    public void CandidatesAreSeparatedByContainerType()
    {
        _presenter.GetCandidates().Should().BeEmpty(because: "nothing is selected yet");

        Select(Root);
        _presenter.GetCandidates().Select(x => x?.NodeType).Should().Equal("Group", null, "Contact");

        Select(_presenter.GetNodes(Root.Target)[0]);
        _presenter.GetCandidates().Select(x => x?.NodeType).Should().Equal("Contact");
    }

    [Fact]
    public void AddSelectsNewNode()
    {
        Select(Root);
        _presenter.Add(_presenter.GetCandidates().First(x => x?.NodeType == "Group")!);

        _structureChangedCount.Should().Be(1);
        _addressBook.Groups.Should().HaveCount(2);
        var newNode = _presenter.GetNodes(Root.Target).Single(x => x.Target == _addressBook.Groups[1]);
        _presenter.ShouldSelect(newNode).Should().BeTrue();
        _presenter.ShouldSelect(Root).Should().BeFalse();

        _presenter.CommandManager.Undo();
        _structureChangedCount.Should().Be(2);
        _addressBook.Groups.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveCommand()
    {
        _presenter.GetRemoveCommand().Should().BeNull(because: "nothing is selected yet");

        Select(Root);
        _presenter.GetRemoveCommand().Should().BeNull(because: "the root cannot be removed");

        Select(_presenter.GetNodes(Root.Target)[0]);
        _presenter.CommandManager.Execute(_presenter.GetRemoveCommand()!);
        _addressBook.Groups.Should().BeEmpty();
    }

    [Fact]
    public void SelectReportsRequiredUpdates()
    {
        var group = _presenter.GetNodes(Root.Target)[0];

        _presenter.Select(group, out bool editorChanged, out bool textChanged);
        editorChanged.Should().BeTrue();
        textChanged.Should().BeTrue();
        _presenter.SelectedNode.Should().BeSameAs(group);
        _presenter.SelectedTarget.Should().BeSameAs(_addressBook.Groups[0]);

        // Same target again, e.g., after the tree was rebuilt
        _presenter.Select(_presenter.GetNodes(Root.Target)[0], out editorChanged, out textChanged);
        editorChanged.Should().BeFalse(because: "the existing editor control is still showing the same target");
        textChanged.Should().BeTrue(because: "the target may have been modified via the editor control");
    }

    [Fact]
    public void UpdateFromText()
    {
        Select(_presenter.GetNodes(Root.Target)[0]);
        string text = _presenter.GetSerialized();
        text.Should().Contain("Scientists");

        _presenter.UpdateFromText(text).Should().BeFalse(because: "the text is unchanged");
        _structureChangedCount.Should().Be(0);

        _presenter.UpdateFromText(text.Replace("Scientists", "Physicists")).Should().BeTrue();
        _structureChangedCount.Should().Be(1);
        _addressBook.Groups[0].Name.Should().Be("Physicists");

        // The tree is rebuilt and the replaced node is selected again
        var newNode = _presenter.GetNodes(Root.Target)[0];
        _presenter.ShouldSelect(newNode).Should().BeTrue();
        _presenter.Select(newNode, out bool editorChanged, out bool textChanged);
        editorChanged.Should().BeTrue(because: "the target object was replaced");
        textChanged.Should().BeFalse(because: "the text the user is typing must not be replaced");
    }

    [Fact]
    public void OpenAdoptsSerializer()
    {
        _presenter.Serializer.Should().BeSameAs(XmlStructureSerializer.Instance);

        var serializer = new NewtonsoftJsonStructureSerializer();
        _presenter.Open(new SerializingCommandManager<AddressBook>(new AddressBook(), serializer));

        _presenter.Serializer.Should().BeSameAs(serializer);
        Root.Target.Should().BeSameAs(_presenter.CommandManager.Target);
    }
}
