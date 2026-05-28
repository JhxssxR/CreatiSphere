import re

filepath = r"c:\IT13_CreatiSphere\Views\Sales\SalesDashboardPage.xaml"

hover_style = """
    <ContentPage.Resources>
        <Style x:Key="HoverCardStyle" TargetType="Border">
            <Setter Property="VisualStateManager.VisualStateGroups">
                <VisualStateGroupList>
                    <VisualStateGroup x:Name="CommonStates">
                        <VisualState x:Name="Normal">
                            <VisualState.Setters>
                                <Setter Property="Stroke" Value="#E2E8F0" />
                                <Setter Property="BackgroundColor" Value="White" />
                                <Setter Property="TranslationY" Value="0" />
                            </VisualState.Setters>
                        </VisualState>
                        <VisualState x:Name="PointerOver">
                            <VisualState.Setters>
                                <Setter Property="Stroke" Value="#0D9488" />
                                <Setter Property="BackgroundColor" Value="#F8FAFC" />
                                <Setter Property="TranslationY" Value="-4" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateGroupList>
            </Setter>
        </Style>
    </ContentPage.Resources>
"""

new_content = """        <!-- ===== MAIN CONTENT ===== -->
        <ScrollView Grid.Column="1" VerticalScrollBarVisibility="Never">
            <VerticalStackLayout Padding="40" Spacing="32">
                
                <!-- Header -->
                <VerticalStackLayout Spacing="8">
                    <Label Text="Sales Dashboard" TextColor="#0F172A" FontSize="28" FontAttributes="Bold" />
                    <Label Text="Monitor your sales performance and customer interactions" TextColor="#64748B" FontSize="14" />
                </VerticalStackLayout>

                <!-- Stats Row -->
                <Grid ColumnDefinitions="*, *, *, *" ColumnSpacing="20">
                    <!-- Customers Handled -->
                    <Border Grid.Column="0" Style="{StaticResource HoverCardStyle}" StrokeShape="RoundRectangle 12" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="24">
                        <VerticalStackLayout Spacing="24">
                            <Grid ColumnDefinitions="*, Auto">
                                <Label Text="Customers Handled" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" VerticalOptions="Center" />
                                <Path Grid.Column="1" Data="M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z" Fill="#94A3B8" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center" />
                            </Grid>
                            <VerticalStackLayout Spacing="4">
                                <Label x:Name="CustomersHandledLabel" Text="234" TextColor="#0F172A" FontSize="28" FontAttributes="Bold" />
                                <Label Text="This month" TextColor="#94A3B8" FontSize="12" />
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Border>
                    
                    <!-- Transactions -->
                    <Border Grid.Column="1" Style="{StaticResource HoverCardStyle}" StrokeShape="RoundRectangle 12" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="24">
                        <VerticalStackLayout Spacing="24">
                            <Grid ColumnDefinitions="*, Auto">
                                <Label Text="Transactions" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" VerticalOptions="Center" />
                                <Path Grid.Column="1" Data="M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l.03-.12.9-1.63h7.45c.75 0 1.41-.41 1.75-1.03l3.58-6.49c.08-.14.12-.31.12-.48 0-.55-.45-1-1-1H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-.9-2-2-2z" Fill="#94A3B8" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center" />
                            </Grid>
                            <VerticalStackLayout Spacing="4">
                                <Label x:Name="TransactionsLabel" Text="487" TextColor="#0F172A" FontSize="28" FontAttributes="Bold" />
                                <Label Text="Processed" TextColor="#94A3B8" FontSize="12" />
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Border>
                    
                    <!-- Sales Revenue -->
                    <Border Grid.Column="2" Style="{StaticResource HoverCardStyle}" StrokeShape="RoundRectangle 12" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="24">
                        <VerticalStackLayout Spacing="24">
                            <Grid ColumnDefinitions="*, Auto">
                                <Label Text="Sales Revenue" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" VerticalOptions="Center" />
                                <Path Grid.Column="1" Data="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z" Fill="#94A3B8" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center" />
                            </Grid>
                            <VerticalStackLayout Spacing="4">
                                <Label x:Name="TotalRevenueLabel" Text="$12,845" TextColor="#0F172A" FontSize="28" FontAttributes="Bold" />
                                <Label Text="Total sales" TextColor="#94A3B8" FontSize="12" />
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Border>
                    
                    <!-- Conversion Rate -->
                    <Border Grid.Column="3" Style="{StaticResource HoverCardStyle}" StrokeShape="RoundRectangle 12" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="24">
                        <VerticalStackLayout Spacing="24">
                            <Grid ColumnDefinitions="*, Auto">
                                <Label Text="Conversion Rate" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" VerticalOptions="Center" />
                                <Path Grid.Column="1" Data="M16 6l2.29 2.29-4.88 4.88-4-4L2 16.59 3.41 18l6-6 4 4 6.3-6.29L22 12V6z" Fill="#94A3B8" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center" />
                            </Grid>
                            <VerticalStackLayout Spacing="4">
                                <Label x:Name="ConversionRateLabel" Text="68%" TextColor="#0F172A" FontSize="28" FontAttributes="Bold" />
                                <Label Text="+5% from last month" TextColor="#94A3B8" FontSize="12" />
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Border>
                </Grid>

                <Grid ColumnDefinitions="*, *" ColumnSpacing="24">
                    <!-- Left Section: Recent Customer Interactions -->
                    <Border Grid.Column="0" StrokeShape="RoundRectangle 12" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="24">
                        <VerticalStackLayout Spacing="20">
                            <Label Text="Recent Customer Interactions" TextColor="#0F172A" FontSize="16" FontAttributes="Bold" />
                            
                            <!-- Items List -->
                            <VerticalStackLayout Spacing="16">
                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="Alice Johnson" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="Purchase completed" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="5 min ago" TextColor="#94A3B8" FontSize="12" VerticalOptions="Center" />
                                    </Grid>
                                </Border>

                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="Bob Smith" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="Inquiry about custom order" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="1 hour ago" TextColor="#94A3B8" FontSize="12" VerticalOptions="Center" />
                                    </Grid>
                                </Border>

                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="Carol White" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="Payment processed" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="2 hours ago" TextColor="#94A3B8" FontSize="12" VerticalOptions="Center" />
                                    </Grid>
                                </Border>

                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="David Brown" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="Order placed" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="3 hours ago" TextColor="#94A3B8" FontSize="12" VerticalOptions="Center" />
                                    </Grid>
                                </Border>
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Border>

                    <!-- Right Section: Top Products Sold -->
                    <Border Grid.Column="1" StrokeShape="RoundRectangle 12" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="24">
                        <VerticalStackLayout Spacing="20">
                            <Label Text="Top Products Sold" TextColor="#0F172A" FontSize="16" FontAttributes="Bold" />
                            
                            <!-- Items List -->
                            <VerticalStackLayout Spacing="16">
                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="Kawaii Sticker Set" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="45 units sold" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="$450" TextColor="#0F172A" FontSize="16" FontAttributes="Bold" VerticalOptions="Center" />
                                    </Grid>
                                </Border>

                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="Meme Templates" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="38 units sold" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="$570" TextColor="#0F172A" FontSize="16" FontAttributes="Bold" VerticalOptions="Center" />
                                    </Grid>
                                </Border>

                                <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#E2E8F0" BackgroundColor="White" Padding="16" Style="{StaticResource HoverCardStyle}">
                                    <Grid ColumnDefinitions="*, Auto">
                                        <VerticalStackLayout Spacing="4">
                                            <Label Text="Digital Art Pack" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                            <Label Text="31 units sold" TextColor="#64748B" FontSize="13" />
                                        </VerticalStackLayout>
                                        <Label Grid.Column="1" Text="$775" TextColor="#0F172A" FontSize="16" FontAttributes="Bold" VerticalOptions="Center" />
                                    </Grid>
                                </Border>
                            </VerticalStackLayout>
                        </VerticalStackLayout>
                    </Border>
                </Grid>

            </VerticalStackLayout>
        </ScrollView>"""

with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

# Add hover style if missing
if "HoverCardStyle" not in content:
    content = re.sub(r'(BackgroundColor="[^"]*">)', r'\1\n' + hover_style, content, count=1)

# Replace the ScrollView
content = re.sub(r'<!-- ===== MAIN CONTENT ===== -->\s*<ScrollView.*?<\/ScrollView>', new_content, content, flags=re.DOTALL)

with open(filepath, 'w', encoding='utf-8') as f:
    f.write(content)
