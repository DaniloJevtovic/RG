   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    AssimpSample   AssimpSample.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	   -   5          � Title$  SharpGL WPF Application��5   	   $	��610��6)   $	��992��66    � WindowStartupLocation$ CenterScreen=�5   a    � WindowState$ 	Maximized=�6�   .��5      ]� .+�5   
   c� $	��Top=�6   .@�5      �� .��6   Transliranje lijevog zida6/   �� 5	      -   5         ��Name$ transSlider��5	   !   $	��100��6    � Minimum$
 -100Q�66    � Maximum$	 100Q�6E    � Value$	 -65Q�6S   6b   �� 5
      -   5        $��translacijaZida��5
      $��50��6-    �� TextAlignment$ Right=�6~    �� 
IsReadOnly . ��68   �� 	 �� ElementName$	 transSlider��
 �� Path$
 Value� �� StringFormat$ N2��6�   �� 5      .��6   Rotiranje desnog zida6+   �� 5      -   5        $ 	rotSlider��5   !   $	��100��6   $
 -360Q�64   $	 360Q�6C   $ 0Q�6Q   6^   �� 5      -   5        $��rotacijaZida��5      $��50��6*   $ Right=�6y    . ��65   �� $	 	rotSlider��$
 Value�$ N2��6�   �� 5      .��6   Skaliranje strijele6)   �� 5      -   5        $ 
skalSlider��5   !   $	��100��6   $	 300Q�65   $ 30Q�6C   6P   �� 5      -   5        $��skalStrijele��5      $��50��6*   $ Right=�6z    . ��65   �� $	 
skalSlider��$
 Value�$ N2��6�   5         SharpGL.WPF.OpenGLControl  5   
   -   5        $��openGLControl��5         DrawFPS .    RenderContextType$	 FBO=�5         	FrameRate$ 10Q�64   5      5      