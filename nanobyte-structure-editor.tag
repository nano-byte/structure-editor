<?xml version='1.0' encoding='UTF-8' standalone='yes' ?>
<tagfile doxygen_version="1.9.1" doxygen_gitid="ef9b20ac7f8a8621fcfc299f8bd0b80422390f4b">
  <compound kind="class">
    <name>NanoByte::StructureEditor::ContainerDescription</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_container_description.html</filename>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::IContainerDescription</base>
    <member kind="function">
      <type>IContainerDescription&lt; TContainer &gt;</type>
      <name>AddProperty&lt; TProperty, TEditor &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_container_description.html</anchorfile>
      <anchor>acdedf246bbc8e2ce878896b6ff6e5f6a</anchor>
      <arglist>(string name, Func&lt; TContainer, PropertyPointer&lt; TProperty?&gt;&gt; getPointer, TEditor editor)</arglist>
    </member>
    <member kind="function">
      <type>IListDescription&lt; TContainer, TList &gt;</type>
      <name>AddList&lt; TList &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_container_description.html</anchorfile>
      <anchor>a7b3696d1119ddedb022884819bdc1c4d</anchor>
      <arglist>(Func&lt; TContainer, IList&lt; TList &gt;&gt; getList)</arglist>
    </member>
    <member kind="function">
      <type>IContainerDescription&lt; TContainer &gt;</type>
      <name>AddPlainList&lt; TElement, TEditor &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_container_description.html</anchorfile>
      <anchor>a6248e62301bb02204813503eb09642cc</anchor>
      <arglist>(string name, Func&lt; TContainer, IList&lt; TElement &gt;&gt; getList, TEditor editor)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; Node &gt;</type>
      <name>GetNodesIn</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_container_description.html</anchorfile>
      <anchor>a2ee84aee4ea4b51a47b6a2cc22e79010</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; NodeCandidate?&gt;</type>
      <name>GetCandidatesFor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_container_description.html</anchorfile>
      <anchor>ac7b468d720069835c84d4d675343dc38</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::ContainerDescriptionExtensions</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_container_description_extensions.html</filename>
    <member kind="function" static="yes">
      <type>static IContainerDescription&lt; TContainer &gt;</type>
      <name>AddProperty&lt; TContainer, TProperty &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_container_description_extensions.html</anchorfile>
      <anchor>a4a3881c2060d5ca73678baf7862a3260</anchor>
      <arglist>(this IContainerDescription&lt; TContainer &gt; description, string name, Func&lt; TContainer, PropertyPointer&lt; TProperty?&gt;&gt; getPointer)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IContainerDescription&lt; TContainer &gt;</type>
      <name>AddPlainList&lt; TContainer, TElement &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_container_description_extensions.html</anchorfile>
      <anchor>ab911275d02dae97a6ea80647194562ea</anchor>
      <arglist>(this IContainerDescription&lt; TContainer &gt; description, string name, Func&lt; TContainer, IList&lt; TElement &gt;&gt; getList)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static IListDescription&lt; TContainer, TList &gt;</type>
      <name>AddElement&lt; TContainer, TList, TElement &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_container_description_extensions.html</anchorfile>
      <anchor>abf09b676c7f4aed146f0464e78cd5b87</anchor>
      <arglist>(this IListDescription&lt; TContainer, TList &gt; description, string name, TElement element)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::Description</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_description.html</filename>
    <templarg></templarg>
    <member kind="function" virtualness="pure">
      <type>abstract IEnumerable&lt; Node &gt;</type>
      <name>GetNodesIn</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_description.html</anchorfile>
      <anchor>add16e84b46c658d9c8518e1b3b8e6eeb</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
    <member kind="function" virtualness="pure">
      <type>abstract IEnumerable&lt; NodeCandidate &gt;</type>
      <name>GetCandidatesFor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_description.html</anchorfile>
      <anchor>ab9694aacce1cb3c81e1a8c47059cde85</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::StructureEditor::IContainerDescription</name>
    <filename>interface_nano_byte_1_1_structure_editor_1_1_i_container_description.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>IContainerDescription&lt; TContainer &gt;</type>
      <name>AddProperty&lt; TProperty, TEditor &gt;</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_container_description.html</anchorfile>
      <anchor>ab347263fb3046eea0694a2e177841497</anchor>
      <arglist>(string name, Func&lt; TContainer, PropertyPointer&lt; TProperty?&gt;&gt; getPointer, TEditor editor)</arglist>
    </member>
    <member kind="function">
      <type>IListDescription&lt; TContainer, TList &gt;</type>
      <name>AddList&lt; TList &gt;</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_container_description.html</anchorfile>
      <anchor>aa99eadf670fa34eb8296e7316714662f</anchor>
      <arglist>(Func&lt; TContainer, IList&lt; TList &gt;&gt; getList)</arglist>
    </member>
    <member kind="function">
      <type>IContainerDescription&lt; TContainer &gt;</type>
      <name>AddPlainList&lt; TElement, TEditor &gt;</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_container_description.html</anchorfile>
      <anchor>ab9ca27ae8015ffae63424c0b8e4e5734</anchor>
      <arglist>(string name, Func&lt; TContainer, IList&lt; TElement &gt;&gt; getList, TEditor editor)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; Node &gt;</type>
      <name>GetNodesIn</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_container_description.html</anchorfile>
      <anchor>a06abf3b90e2dff81c2c92ad1131b083c</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
    <member kind="function">
      <type>IEnumerable&lt; NodeCandidate?&gt;</type>
      <name>GetCandidatesFor</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_container_description.html</anchorfile>
      <anchor>a9093ebc537b82233dac09c0080e6ac48</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::StructureEditor::IListDescription</name>
    <filename>interface_nano_byte_1_1_structure_editor_1_1_i_list_description.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <member kind="function">
      <type>IListDescription&lt; TContainer, TList &gt;</type>
      <name>AddElement&lt; TElement, TEditor &gt;</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_list_description.html</anchorfile>
      <anchor>aac1eb8d113d01b98cf8f1925d17883af</anchor>
      <arglist>(string name, TElement element, TEditor editor)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::Resources::Images</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</filename>
    <member kind="property" protection="package" static="yes">
      <type>static global::System.Resources.ResourceManager</type>
      <name>ResourceManager</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</anchorfile>
      <anchor>adcd972fbd574b2c7141f9f0a445f62a0</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="package" static="yes">
      <type>static global::System.Globalization.CultureInfo</type>
      <name>Culture</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</anchorfile>
      <anchor>a8e6df3b92e312e68648a672ca805d605</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="package" static="yes">
      <type>static System.Drawing.Bitmap</type>
      <name>AddButton</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</anchorfile>
      <anchor>ad826c2df4f8e59ee47516b2213e43d66</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="package" static="yes">
      <type>static System.Drawing.Bitmap</type>
      <name>DeleteButton</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</anchorfile>
      <anchor>a9d2a23fa87d5b267aebbed14e7a96179</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="package" static="yes">
      <type>static System.Drawing.Bitmap</type>
      <name>Error</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</anchorfile>
      <anchor>af34ed381d6ccf5de44a80d1cc7fd6add</anchor>
      <arglist></arglist>
    </member>
    <member kind="property" protection="package" static="yes">
      <type>static System.Drawing.Bitmap</type>
      <name>Info</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources_1_1_images.html</anchorfile>
      <anchor>a436fdd23f7c31565f4cbd702f4784852</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::StructureEditor::INodeEditor</name>
    <filename>interface_nano_byte_1_1_structure_editor_1_1_i_node_editor.html</filename>
    <templarg></templarg>
    <member kind="property">
      <type>T?</type>
      <name>Target</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_node_editor.html</anchorfile>
      <anchor>a8fccc52031bf22be84257da7ee7e8993</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::StructureEditor::IStructureEditor</name>
    <filename>interface_nano_byte_1_1_structure_editor_1_1_i_structure_editor.html</filename>
    <templarg></templarg>
    <member kind="function">
      <type>IContainerDescription&lt; TContainer &gt;</type>
      <name>Describe&lt; TContainer &gt;</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_structure_editor.html</anchorfile>
      <anchor>af315b5f2b21af3a58eee068e33604ada</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>IContainerDescription&lt; T &gt;</type>
      <name>DescribeRoot</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_structure_editor.html</anchorfile>
      <anchor>a603b3e3a0a90933da59fb26a66747b30</anchor>
      <arglist>(string name)</arglist>
    </member>
    <member kind="function">
      <type>IContainerDescription&lt; T &gt;</type>
      <name>DescribeRoot&lt; TEditor &gt;</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_structure_editor.html</anchorfile>
      <anchor>ad50fc00c82c992281d3e5f96bb343a65</anchor>
      <arglist>(string name)</arglist>
    </member>
  </compound>
  <compound kind="interface">
    <name>NanoByte::StructureEditor::ITargetContainerInject</name>
    <filename>interface_nano_byte_1_1_structure_editor_1_1_i_target_container_inject.html</filename>
    <templarg></templarg>
    <member kind="property">
      <type>T?</type>
      <name>TargetContainer</name>
      <anchorfile>interface_nano_byte_1_1_structure_editor_1_1_i_target_container_inject.html</anchorfile>
      <anchor>a985bd1a57075c071ac9889690d6792ca</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::ListDescription</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_list_description.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::Description</base>
    <base>NanoByte::StructureEditor::IListDescription</base>
    <member kind="function">
      <type></type>
      <name>ListDescription</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_description.html</anchorfile>
      <anchor>aa008815ccc16d09094569dd31d31ecd4</anchor>
      <arglist>(Func&lt; TContainer, IList&lt; TList &gt;&gt; getList)</arglist>
    </member>
    <member kind="function">
      <type>override IEnumerable&lt; Node &gt;</type>
      <name>GetNodesIn</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_description.html</anchorfile>
      <anchor>aba9edf8fea6c6ede3bbcdbceff058452</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
    <member kind="function">
      <type>override IEnumerable&lt; NodeCandidate &gt;</type>
      <name>GetCandidatesFor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_description.html</anchorfile>
      <anchor>a41c4ca896939f960901df5540d70de82</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
    <member kind="function">
      <type>IListDescription&lt; TContainer, TList &gt;</type>
      <name>AddElement&lt; TElement, TEditor &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_description.html</anchorfile>
      <anchor>a57799ebc67868595ac12c3f817cc8a49</anchor>
      <arglist>(string name, TElement element, TEditor editor)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::ListElementNode</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_list_element_node.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <templarg></templarg>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::Node</base>
    <member kind="function">
      <type></type>
      <name>ListElementNode</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node.html</anchorfile>
      <anchor>a2fb60eea4699c62d13cbf97e587929b4</anchor>
      <arglist>(string name, TContainer container, IList&lt; TList &gt; list, TElement element)</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>GetSerialized</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node.html</anchorfile>
      <anchor>ab0d24d754ff67c3d90e22572b400f78a</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override? IValueCommand</type>
      <name>GetUpdateCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node.html</anchorfile>
      <anchor>ad9ad85605bfa4feff021c4e6b135e5fe</anchor>
      <arglist>(string serializedValue)</arglist>
    </member>
    <member kind="function">
      <type>override IUndoCommand</type>
      <name>GetRemoveCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node.html</anchorfile>
      <anchor>aa3c6d12e9c48380e506742ecc585f675</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override INodeEditor</type>
      <name>GetEditorControl</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node.html</anchorfile>
      <anchor>addf9010f8130dcb29d94f3d0cb773fcf</anchor>
      <arglist>(ICommandExecutor executor)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::ListElementNodeCandidate</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_list_element_node_candidate.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::NodeCandidate</base>
    <member kind="function">
      <type></type>
      <name>ListElementNodeCandidate</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node_candidate.html</anchorfile>
      <anchor>a89f0f5650a80e56edbc274325a1b946d</anchor>
      <arglist>(string name, IList&lt; TList &gt; list)</arglist>
    </member>
    <member kind="function">
      <type>override IValueCommand</type>
      <name>GetCreateCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_list_element_node_candidate.html</anchorfile>
      <anchor>a392d781be2a60831418fcf29d1fa8a5d</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::LocalizableTextBox</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_localizable_text_box.html</filename>
    <base>NodeEditorBase&lt; LocalizableStringCollection &gt;</base>
    <member kind="property">
      <type>bool</type>
      <name>Multiline</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_localizable_text_box.html</anchorfile>
      <anchor>a9a271b2ac636b5b8032daf44bb8e5962</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>HintText</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_localizable_text_box.html</anchorfile>
      <anchor>aac4064527084aef067dce9426feb982b</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::Node</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_node.html</filename>
    <member kind="function" virtualness="pure">
      <type>abstract string</type>
      <name>GetSerialized</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>ac75a1a124e69e2937048c616d86188ad</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="pure">
      <type>abstract ? IValueCommand</type>
      <name>GetUpdateCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>a2cf1826356ddb2f5e0ffabf485e50c52</anchor>
      <arglist>(string serializedValue)</arglist>
    </member>
    <member kind="function" virtualness="pure">
      <type>abstract IUndoCommand</type>
      <name>GetRemoveCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>ac254af3918a5f06af1fd05771d39dc87</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" virtualness="pure">
      <type>abstract INodeEditor</type>
      <name>GetEditorControl</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>a97e3e486655594e618f7122a203b52aa</anchor>
      <arglist>(ICommandExecutor executor)</arglist>
    </member>
    <member kind="function" static="yes">
      <type>static ? string</type>
      <name>GetDescription&lt; T &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>adc24d6648ec236420470a246a561205d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type></type>
      <name>Node</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>ae6300ecfcabb910b5311aeafa15e6ca4</anchor>
      <arglist>(string nodeType, string? description, object? target)</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>NodeType</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>a2162f1c38f343dbf8ef3bc71875f17ac</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Description</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>a76bcf8e5638e21dd9e11b41d1cda5fba</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>object?</type>
      <name>Target</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node.html</anchorfile>
      <anchor>a6b7c2195710e0a8fbb7ae235d455fc0d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::NodeCandidate</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_node_candidate.html</filename>
    <member kind="function" virtualness="pure">
      <type>abstract IValueCommand</type>
      <name>GetCreateCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node_candidate.html</anchorfile>
      <anchor>ac2f7a8584ccf3ba03077678e198c0374</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected">
      <type></type>
      <name>NodeCandidate</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node_candidate.html</anchorfile>
      <anchor>a2ca13ff1bdeecd0f859e2b45b0d121b6</anchor>
      <arglist>(string nodeType, string? description)</arglist>
    </member>
    <member kind="property">
      <type>string</type>
      <name>NodeType</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node_candidate.html</anchorfile>
      <anchor>a73ab206df9d7ce56c37cb2f1000d75fc</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>string?</type>
      <name>Description</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_node_candidate.html</anchorfile>
      <anchor>a6c6e0644c08a6f84a4b69dd93f4ee4ca</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::NodeEditorBase</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</filename>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::INodeEditor</base>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a8d26318734ae8c8a96c750cc3d24a055</anchor>
      <arglist>(Control control, PropertyPointer&lt; string?&gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>af7905d56e24b29adf907a80e1579cbff</anchor>
      <arglist>(ComboBox control, PropertyPointer&lt; string?&gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>ad06023f2cfdc94fadb763967a4a8dba8</anchor>
      <arglist>(UriTextBox control, PropertyPointer&lt; Uri?&gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind&lt; TControl, TChild &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a0e3abf4b03cc8b62f55ed380f9c8e6ff</anchor>
      <arglist>(TControl control, Func&lt; TChild &gt; getTarget)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a7556b820ac4131d8014b1ee8c94169d1</anchor>
      <arglist>(CheckBox control, PropertyPointer&lt; bool &gt; pointer)</arglist>
    </member>
    <member kind="property">
      <type>virtual ? T?</type>
      <name>Target</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>af3d8b0d054bec339a407d265beb3d6d8</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>virtual ? ICommandExecutor?</type>
      <name>CommandExecutor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>ab244452a98636b158918c193eac5e7cc</anchor>
      <arglist></arglist>
    </member>
    <member kind="event" protection="protected">
      <type>Action?</type>
      <name>TargetChanged</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>aa8ee6d20d6ab5d4e07f0f8e84cd75e70</anchor>
      <arglist></arglist>
    </member>
    <member kind="event" protection="protected">
      <type>Action?</type>
      <name>CommandExecutorChanged</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>aa38d5610a00fc261512c5b39f8a343aa</anchor>
      <arglist></arglist>
    </member>
    <member kind="event" protection="protected">
      <type>Action?</type>
      <name>OnRefresh</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a8a51f49d79a6e4e5ca5edfa6f9e9856d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NodeEditorBase&lt; LocalizableStringCollection &gt;</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</filename>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a8d26318734ae8c8a96c750cc3d24a055</anchor>
      <arglist>(Control control, PropertyPointer&lt; string?&gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>af7905d56e24b29adf907a80e1579cbff</anchor>
      <arglist>(ComboBox control, PropertyPointer&lt; string?&gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>ad06023f2cfdc94fadb763967a4a8dba8</anchor>
      <arglist>(UriTextBox control, PropertyPointer&lt; Uri?&gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a7556b820ac4131d8014b1ee8c94169d1</anchor>
      <arglist>(CheckBox control, PropertyPointer&lt; bool &gt; pointer)</arglist>
    </member>
    <member kind="function" protection="protected">
      <type>void</type>
      <name>Bind&lt; TControl, TChild &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a0e3abf4b03cc8b62f55ed380f9c8e6ff</anchor>
      <arglist>(TControl control, Func&lt; TChild &gt; getTarget)</arglist>
    </member>
    <member kind="property">
      <type>virtual ? T?</type>
      <name>Target</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>af3d8b0d054bec339a407d265beb3d6d8</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>virtual ? ICommandExecutor?</type>
      <name>CommandExecutor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>ab244452a98636b158918c193eac5e7cc</anchor>
      <arglist></arglist>
    </member>
    <member kind="event" protection="protected">
      <type>Action?</type>
      <name>TargetChanged</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>aa8ee6d20d6ab5d4e07f0f8e84cd75e70</anchor>
      <arglist></arglist>
    </member>
    <member kind="event" protection="protected">
      <type>Action?</type>
      <name>CommandExecutorChanged</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>aa38d5610a00fc261512c5b39f8a343aa</anchor>
      <arglist></arglist>
    </member>
    <member kind="event" protection="protected">
      <type>Action?</type>
      <name>OnRefresh</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_node_editor_base.html</anchorfile>
      <anchor>a8a51f49d79a6e4e5ca5edfa6f9e9856d</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::PropertyDescription</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_property_description.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::Description</base>
    <member kind="function">
      <type></type>
      <name>PropertyDescription</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_description.html</anchorfile>
      <anchor>a8c2b79a82decd099bd6fd7951bfe9d3c</anchor>
      <arglist>(string name, Func&lt; TContainer, PropertyPointer&lt; TProperty?&gt;&gt; getPointer)</arglist>
    </member>
    <member kind="function">
      <type>override IEnumerable&lt; Node &gt;</type>
      <name>GetNodesIn</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_description.html</anchorfile>
      <anchor>ac5baee6679e59263d74564a956f1b2a5</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
    <member kind="function">
      <type>override IEnumerable&lt; NodeCandidate &gt;</type>
      <name>GetCandidatesFor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_description.html</anchorfile>
      <anchor>a210416531f89af340d501effb1bfac95</anchor>
      <arglist>(TContainer container)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::PropertyGridNodeEditor</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_property_grid_node_editor.html</filename>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::WinForms::NodeEditorBase</base>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::PropertyNode</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_property_node.html</filename>
    <templarg></templarg>
    <templarg></templarg>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::Node</base>
    <member kind="function">
      <type></type>
      <name>PropertyNode</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node.html</anchorfile>
      <anchor>a18dbbf30ea045d8cc9e8956768cd01c1</anchor>
      <arglist>(string name, TContainer container, PropertyPointer&lt; TProperty?&gt; pointer)</arglist>
    </member>
    <member kind="function">
      <type>override string</type>
      <name>GetSerialized</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node.html</anchorfile>
      <anchor>aa1a486488c5e36544c14e98a20b0c39f</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override? IValueCommand</type>
      <name>GetUpdateCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node.html</anchorfile>
      <anchor>af830dc1c2fb52f233d2e25a71baa9ad1</anchor>
      <arglist>(string serializedValue)</arglist>
    </member>
    <member kind="function">
      <type>override IUndoCommand</type>
      <name>GetRemoveCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node.html</anchorfile>
      <anchor>ad1630c9c66ba629649653c5cc5557811</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>override INodeEditor</type>
      <name>GetEditorControl</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node.html</anchorfile>
      <anchor>aaab0e1fb0cb54620482c785984bbdf79</anchor>
      <arglist>(ICommandExecutor executor)</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::PropertyNodeCandidate</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_property_node_candidate.html</filename>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::NodeCandidate</base>
    <member kind="function">
      <type></type>
      <name>PropertyNodeCandidate</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node_candidate.html</anchorfile>
      <anchor>a001313b31984e2255f7a82c49b9fd841</anchor>
      <arglist>(string name, PropertyPointer&lt; TProperty?&gt; pointer)</arglist>
    </member>
    <member kind="function">
      <type>override IValueCommand</type>
      <name>GetCreateCommand</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_property_node_candidate.html</anchorfile>
      <anchor>a9264a4690882461baf0747f923ac1c8d</anchor>
      <arglist>()</arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::StructureEditor</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</filename>
    <templarg></templarg>
    <base>NanoByte::StructureEditor::IStructureEditor</base>
    <member kind="function">
      <type>IContainerDescription&lt; TContainer &gt;</type>
      <name>Describe&lt; TContainer &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>ac7da911fbece6a77f838863f717f3351</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>IContainerDescription&lt; T &gt;</type>
      <name>DescribeRoot&lt; TEditor &gt;</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>a701c9eeb3ce78b3e996ce68670b04cb2</anchor>
      <arglist>(string name)</arglist>
    </member>
    <member kind="function">
      <type>IContainerDescription&lt; T &gt;</type>
      <name>DescribeRoot</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>a97a20237f8e5a1bc8cce7a7c8325b8b5</anchor>
      <arglist>(string name)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Open</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>a8f8b0fb68c2e0a15d62bf9f031447be1</anchor>
      <arglist>(ICommandManager&lt; T &gt; commandManager)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Undo</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>afb050723fd0d3ad76b8c2a721616793d</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Redo</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>a8d72a504c0934fb8c89ad02eccc5b4ab</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Remove</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>a62692428da51bf98bb4a17c3528e656c</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function" protection="protected" virtualness="virtual">
      <type>virtual string</type>
      <name>GetSerialized</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>af30f01276e2c754f23b1054f572eb85e</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>ICommandManager&lt; T &gt;</type>
      <name>CommandManager</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_editor.html</anchorfile>
      <anchor>a0cc42a5ebadcd4398d33f5a506459ebc</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::StructureTreeNode</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_structure_tree_node.html</filename>
  </compound>
  <compound kind="class">
    <name>NanoByte::StructureEditor::WinForms::ValidatingTextEditor</name>
    <filename>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</filename>
    <member kind="function">
      <type>void</type>
      <name>SetContent</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>a56e337d4a84a40817020e98825f4c72b</anchor>
      <arglist>(string text, string format)</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>ClearUndoStack</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>ac2ca9d4709218b78ea13cb5b74e77479</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Undo</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>a1f4e3eedd201601a40cf9c0e32f603a0</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="function">
      <type>void</type>
      <name>Redo</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>a284cae5c7c281b7c5e3617ffadb96fa0</anchor>
      <arglist>()</arglist>
    </member>
    <member kind="property">
      <type>TextEditorControl</type>
      <name>TextEditor</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>adaa29d0fa51cde03c8f3f2b8ad3dd149</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>EnableUndo</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>ac1f8818e8066b3328ad451637c821987</anchor>
      <arglist></arglist>
    </member>
    <member kind="property">
      <type>bool</type>
      <name>EnableRedo</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>aa09e67d3f6c523b268a7b865170085e9</anchor>
      <arglist></arglist>
    </member>
    <member kind="event">
      <type>Action&lt; string &gt;?</type>
      <name>ContentChanged</name>
      <anchorfile>class_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_validating_text_editor.html</anchorfile>
      <anchor>a82857588e69ae544f3bc5eb46d495696</anchor>
      <arglist></arglist>
    </member>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::StructureEditor</name>
    <filename>namespace_nano_byte_1_1_structure_editor.html</filename>
    <namespace>NanoByte::StructureEditor::WinForms</namespace>
    <class kind="class">NanoByte::StructureEditor::ContainerDescription</class>
    <class kind="class">NanoByte::StructureEditor::Description</class>
    <class kind="interface">NanoByte::StructureEditor::IContainerDescription</class>
    <class kind="interface">NanoByte::StructureEditor::IListDescription</class>
    <class kind="interface">NanoByte::StructureEditor::INodeEditor</class>
    <class kind="interface">NanoByte::StructureEditor::IStructureEditor</class>
    <class kind="interface">NanoByte::StructureEditor::ITargetContainerInject</class>
    <class kind="class">NanoByte::StructureEditor::ListDescription</class>
    <class kind="class">NanoByte::StructureEditor::ListElementNode</class>
    <class kind="class">NanoByte::StructureEditor::ListElementNodeCandidate</class>
    <class kind="class">NanoByte::StructureEditor::Node</class>
    <class kind="class">NanoByte::StructureEditor::NodeCandidate</class>
    <class kind="class">NanoByte::StructureEditor::PropertyDescription</class>
    <class kind="class">NanoByte::StructureEditor::PropertyNode</class>
    <class kind="class">NanoByte::StructureEditor::PropertyNodeCandidate</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::StructureEditor::WinForms</name>
    <filename>namespace_nano_byte_1_1_structure_editor_1_1_win_forms.html</filename>
    <namespace>NanoByte::StructureEditor::WinForms::Resources</namespace>
    <class kind="class">NanoByte::StructureEditor::WinForms::ContainerDescriptionExtensions</class>
    <class kind="class">NanoByte::StructureEditor::WinForms::LocalizableTextBox</class>
    <class kind="class">NanoByte::StructureEditor::WinForms::NodeEditorBase</class>
    <class kind="class">NanoByte::StructureEditor::WinForms::PropertyGridNodeEditor</class>
    <class kind="class">NanoByte::StructureEditor::WinForms::StructureEditor</class>
    <class kind="class">NanoByte::StructureEditor::WinForms::StructureTreeNode</class>
    <class kind="class">NanoByte::StructureEditor::WinForms::ValidatingTextEditor</class>
  </compound>
  <compound kind="namespace">
    <name>NanoByte::StructureEditor::WinForms::Resources</name>
    <filename>namespace_nano_byte_1_1_structure_editor_1_1_win_forms_1_1_resources.html</filename>
    <class kind="class">NanoByte::StructureEditor::WinForms::Resources::Images</class>
  </compound>
  <compound kind="page">
    <name>index</name>
    <title></title>
    <filename>index.html</filename>
    <docanchor file="index.html">md_D__a_structure_editor_structure_editor_doc_main</docanchor>
  </compound>
</tagfile>
