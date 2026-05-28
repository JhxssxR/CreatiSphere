$files = @(
    'Views\Customer\TrackOrdersPage.xaml',
    'Views\Customer\MessagesPage.xaml',
    'Views\Customer\RewardsPage.xaml',
    'Views\Customer\FeedbackPage.xaml',
    'Views\Customer\CustomOrdersPage.xaml',
    'Views\Customer\BrowseMarketplacePage.xaml'
)

$replacementXAML = @"
                        <VerticalStackLayout x:Name=""NotificationsContainer"" Spacing=""0"">
                            <BindableLayout.ItemTemplate>
                                <DataTemplate>
                                    <VerticalStackLayout>
                                        <Grid Padding=""20,16"" ColumnDefinitions=""Auto, *, Auto"">
                                            <Border HeightRequest=""8"" WidthRequest=""8"" StrokeShape=""RoundRectangle 4"" BackgroundColor=""{Binding IconColor}"" VerticalOptions=""Start"" Margin=""0,6,12,0"" StrokeThickness=""0"" />
                                            <VerticalStackLayout Grid.Column=""1"" Spacing=""4"">
                                                <Label Text=""{Binding Title}"" TextColor=""#0F172A"" FontSize=""14"" FontAttributes=""Bold"" />
                                                <Label Text=""{Binding Message}"" TextColor=""#475569"" FontSize=""13"" />
                                                <Label Text=""{Binding TimeAgo}"" TextColor=""#94A3B8"" FontSize=""11"" />
                                            </VerticalStackLayout>
                                            <HorizontalStackLayout Grid.Column=""2"" Spacing=""8"" VerticalOptions=""Start"">
                                                <Border BackgroundColor=""#F1F5F9"" StrokeShape=""RoundRectangle 6"" StrokeThickness=""0"" Padding=""8,4"">
                                                    <Border.GestureRecognizers><TapGestureRecognizer Tapped=""OnMarkAsReadTapped"" /></Border.GestureRecognizers>
                                                    <Label Text=""Read"" TextColor=""#64748B"" FontSize=""11"" FontAttributes=""Bold"" />
                                                </Border>
                                                <Border BackgroundColor=""#FEE2E2"" StrokeShape=""RoundRectangle 6"" StrokeThickness=""0"" Padding=""8,4"">
                                                    <Border.GestureRecognizers><TapGestureRecognizer Tapped=""OnDeleteNotificationTapped"" /></Border.GestureRecognizers>
                                                    <Label Text=""Del"" TextColor=""#EF4444"" FontSize=""11"" FontAttributes=""Bold"" />
                                                </Border>
                                            </HorizontalStackLayout>
                                        </Grid>
                                        <BoxView HeightRequest=""1"" Color=""#F1F5F9"" />
                                    </VerticalStackLayout>
                                </DataTemplate>
                            </BindableLayout.ItemTemplate>
                        </VerticalStackLayout>
"@

foreach ($f in $files) {
    if (Test-Path $f) {
        $content = Get-Content $f -Raw
        $pattern = '(?s)<VerticalStackLayout x:Name="NotificationsContainer" Spacing="0">.*?</VerticalStackLayout>\s*</ScrollView>'
        $newContent = $content -replace $pattern, ($replacementXAML + "`n                    </ScrollView>")
        Set-Content $f $newContent
    }
}
