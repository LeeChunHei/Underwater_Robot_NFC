   M S B A M L   `   `   `  ���� O  KUnderwater_Robot_NFC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Underwater_Robot_NFCUnderwater_Robot_NFC  %    Underwater_Robot_NFC.MainWindow   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:Underwater_Robot_NFC       � Title$!  Underwater Robot NFC system��$	��500��$	��700�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ NoResize=�   MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�   TextRenderingMode$
 Auto=�  MaterialDesignPaper#���    MaterialDesignFont#���  .���  � ColumnDefinitions �� $	��300 ��� $��* �.+�(  "MaterialDesignThemes.Wpf.ColorZone     Mode$ Light=�+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth3=�$��Stretch=�.���� .+��� $��24q�.+���    MaterialDesignTitleTextBlock#���  $��COM Connection��$��
0, 0, 0, 8q��  �� $��* ��� $	��120 �.+��� -    ��Name$ comport_list��  MaterialDesignComboBox#���  $	��120��)  #MaterialDesignThemes.Wpf.HintAssist	   Hint$	 COM Port��
   
IsFloating
 . $��
0, 0, 8, 0q�$��0���� -    ��Name$ connect_btn��$��Connect��$��1���� $��24q�.+��� #���  $��	Card Info��$��
0, 0, 0, 8q��  �� $	��100 ��� $��* � � RowDefinitions �� $��30 ��� $��60 ��� $��30 �.+��� -    ��Name$ LabelTeamID��$��	Team ID: ��$��0��$��0���� -    ��Name$ TextBlockTeamID��$�� �� �� Padding$ 6q�$��0��$��1���� -   $ LabelTeamName��$��Team Name: ��$��1��$��0���� -   $ TextBlockTeamName��$�� ��$ 6q�$��1��$��1��$
��Wrap=��� -   $ label2��$��Credits:��$��2��$��0���� -   $ TextBlockBalance��$ 6q�$��2��$��1���� $��1��.+�  $
 Dark=�$��24q�$ Depth1=�   ShadowEdges$ Bottom=�.���  �� $��80 ��� $��* �.+��� $��Total $��  MaterialDesignLabel#���  
�������$��0��$��Bottom=��� -	   $ TextBlockSum��$��0��$��1�� # MaterialDesignDisplay1TextBlock#���  $��Center=� �� TextAlignment$ Right=�� $��12q� �� $��* ��� $��* �.+�#  MaterialDesignThemes.Wpf.Card  $��4q�$��16, 24q�$��0��.���  �� $��50 ��� $��* ��� $
��Auto �.+��� $��Weight��#���    SecondaryAccentBrush#���  $��0��$��Bottom=��� -
    ��Name$ TextBoxWeight��  MaterialDesignTextBox#���  $��24(�$��Center=�$��0��$��1�� �� TextAlignment$ Right=�$��2��$
��Left=�$��0.4,0,0,-0.4q�$��64���� $��g��$��2��#���  $��Bottom=�  $��4q�$��16, 24q�$��1��.���  �� $��50 ��� $��* ��� $
��Auto �.+��� $��Price��#���  #���  $��0��$��Bottom=��� -   $ TextBoxUnitPrice��#���  $��24(�$��Center=�$��0��$��1��$ Right=��� $	��$/g��$��2��#���  $��Bottom=�� $��16, 0q� �� $	��100 ��� $	��100 ��� $��* � �� $��70 ��� $��70 ��� $��70 �.+��� -   $ price_add_one��$	��+1g��$��4q�$��0��$��1��$
��Auto�� #	 MaterialDesignRaisedLightButton#��� 	 �� -   $ price_add_five��$	��+5g��$��4q�$��1��$��1��$
��Auto��#��� 	 �� -   $ price_add_ten��$
��+10g��$��4q�$��2��$��1��$
��Auto��#��� 	 �� -   $ price_sub_ten��$
��-10g��$��4q�$��2��$��0��$
��Auto��#��� 	 �� -   $ price_sub_five��$	��-5g��$��4q�$��1��$��0��$
��Auto��#��� 	 �� -   $ price_sub_one��$	��-1g��$��4q�$��0��$��0��$
��Auto��#��� 	 �� -   $ checkout��$��Checkout��$��4q�$��1��$��2��$��2��$
��Auto�� $
  MaterialDesignRaisedAccentButton#��� 
 �� -   $ clr_btn��$��Clear��$��0��$��2��$��4q�$
��Auto�� " MaterialDesignRaisedDarkButton#���  