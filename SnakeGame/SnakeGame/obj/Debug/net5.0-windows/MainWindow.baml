   M S B A M L   `   `   `  ����D  @SnakeGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:SnakeGame	SnakeGame  5   	       SnakeGame.MainWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:SnakeGame     5   	   -   5          � Title$  	SnakeGame��5   	   $	��500��6   $	��800��6(   $	��300��5	   	   $	��300��5
   	      BackgroundColor#��[  5   	     	TextColor#��[ 5   	     MainFont#��[ 5   	    � WindowStartupLocation$ CenterScreen=�5   	    � Icon$ Assets/icon.ico��5   	   .��5      (� .��5   
   � .+�5      �  � RowDefinitions 5      �� 5      $
��auto �6$   �� 5      $��* �6$   .+�5      �� -   5         ��Name$ 	ScoreText��5      $��Center=�5      $��Center=�5      $��SCORE 0��5      $��22(�5      $��10q�5      �� 5      -   5         ��Name$ 
GridBorder��5      $��1��5         GridLineColor#��[ 5!      $	��1.5q�5"      $��Center=�5#      $��Center=�5$       �� EdgeMode$ Aliased=�5%      $��
20,0,20,20q�5&      ._�5'      A� -   5         A�Name$ GameGrid��5'   "   $	��400��5(      $	��400��5)        GridBackgroundColor#��[ 5*       J� SnapsToDevicePixels$
 True��5+      	 �� BitmapScalingMode$	 HighQuality=�5,      5-      5.      �� 5/      -   5        $ Overlay��5/      $��1��50        OverlayColor#��[ 51      $ Aliased=�55      ��52      �� 
 �� Path$
 ActualWidth� �� ElementName$ 
GridBorder����53      �� $
 ActualHeight�$ 
GridBorder����54      �� $
 Margin�$ 
GridBorder��._�56      �� -   5        $ OverlayText��56       $ ��PREMI UN TASTO PER GIOCARE��57       $��24(�58       $��Center=�59       $��Center=�5:       $
��Wrap=�5;       5<      5=      � 5@      -   5         �Name$ LeaderboardOverlay��5@      $y�Hidden=�6/   .+�5A      �� ���	#C00000006   �� 5B      #��[  6   #��[ 5C      $��2q�5D       �� CornerRadius$ 10��5E       �� Padding$ 20q�5F      $	��400��5G      $	��500��5H      ._�5I      �� .+�5J      �� $��
CLASSIFICA��6$   $��28(�5K   $   $��Center=�5L   $   $��0,0,0,20q�5M   $   �� 5N      $	��350��6'   $
��Auto=�67   .��5O      �� -   5        $ LeaderboardText��5O   (   $��14(�5P   (    �� TextAlignment$
 Left=�5Q   (   5R      �� 5S      -	   5        $��Chiudi��5S   !   $��Center=�5U   !   $��0,20,0,0q�5V   !   $
��10,5q�5W   !   5X      5Y      5Z      5[      5\      5]      