   M S B A M L   `   `   `  ����I  EPractica_Final, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Practica_FinalPractica_Final  5   	       Practica_Final.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Practica_Final     5   	   -   5          � Title$  Graphinator3000��5   	   $	��450��6!   $	��800��6.   $	��396��6:   $	��680��6J    � WindowState$ 	Maximized=�6z   .��5
      � -   5        $��mainGrid��5
       � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $
��0.5* �6   �� 5      $��3* �6   �� 5      $
��0.5* �6   �� 5      $
��0.5* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   �� -   5        $��
openButton��5      $��0��6#   $��0��60      openButtonStyle#���   6@   $��0q�6j   ��5      �� 5       �� ImageSource$' !Images\openFolder\openFolder1.png��6    j� Stretch$ Uniform=�6M   5!      �� 5"   
   -   5        $��	newButton��5"      $��0��6"   $��1��6/   $��0q�6?     newButtonStyle#���  6S   ��5#      �� 5$      $! Images\newFile\newFile1.png��6   $ Uniform=�6G   5&      �� 5'   
   -   5        $��
saveButton��5'      $��0��6#   $��2��60   $��0q�6@     saveButtonSytle#���  6T   ��5(      �� 5)      $ Images\save\save1.png��6   $ Uniform=�6A   5+      �� 5,   
   -   5        $��exportButton��5,      $��0��6%   $��3��62   $��0q�6B     exportButtonStyle#���  6V   ��5-      �� 5.      $ Uniform=�6   $! Images\export\exportar1.png��6/   50      �� 51   
   -   5        $��dataConfigButton��51      $��0��6)   $��4��66   $��0q�6F     dataConfigButtonStyle#���  6Z   ��52      �� 53      $ Images\table\table1.png��6   $ Uniform=�6C   55      �� 56   
   -   5        $��graficsButton��56      $��0��6&   $��5��63   $��0q�6C     graphicsButtonStyle#���  6W   ��57      �� 58      $# Images\graphics\graphics1.png��6   $ Uniform=�6I   5:      �� 5;   
   -	   5        $��
helpButton��5;      $��0��6#   $��6��60   $��0q�6@     helpButtonStyle#���  6T   ��5<      �� 5=      $ Uniform=�6   $ Images\help\help1.png��6/   5?      �� 5B   
   $��1��6   $��0��6   $��1q�6.   
�������$��0 2 0 2q�6V    �� CornerRadius$ 10��6g   
�������._�5C      �� -
   5        $��tableNumLabel��5C      $��1��6)   $��0��66   $��
Tabla nº0��6F   $��Center=�6Z   $��Center=�6u   $��Impact)�6�   $��20(�6�   
���   �
�������6�   5D      �� 5G   
   $��1��6   $��2��6   $��6��6/   $��1q�6?   
�������$��	10 2 10 2q�6f   $ 10��6y   
�������._�5H      �� $
��Auto=�6   $
��Auto=�6@   .��5I      �� -   5        $��legendStackPanel��5I      
�������$��3q�6L   .+�5J      �� -   5        $��legendLabel��5J      $��Leyenda:��6/   $��Center=�6B   $
��Left=�6]   $��IMPACT)�6x   $��30(�6�   
���   �
�������$��0q�6�   5K      5L      5M      �� 5P   
   $��1��6   $��4��6   $��1��6/   $��5��6?   $��	10 2 10 2q�6S   $��1q�6f   ���#dee2e3}�. ._�5Q      �� -   5        $��grafico��5Q      ���#f5f9fa6y   5R      �� 5X   
   -   5        $��zoomInButton��5X      $��3��6%   $��6��62   $
��37 0q�6B   ���	#00000000  zoomInButtonStyle#���  6i   ��5Y      �� 5Z      $ Uniform=�6   $ Images\zoomIn\zoomIn1.png��6/   5\      �� 5^   
   -   5        $��zoomOutButton��5^      $��4��6&   $��6��63   $
��37 0q�6C   ���	#00000000  zoomOutButtonStyle#���  6i   ��5_      �� 5`      $ Uniform=�6   $! Images\zoomOut\zoomOut1.png��6/   5b      5d      5e      