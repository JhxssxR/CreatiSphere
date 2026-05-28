$hoverStyle = @'
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style TargetType="Border" x:Key="HoverCardStyle">
                <Setter Property="VisualStateManager.VisualStateGroups">
                    <VisualStateGroupList>
                        <VisualStateGroup x:Name="CommonStates">
                            <VisualState x:Name="Normal" />
                            <VisualState x:Name="PointerOver">
                                <VisualState.Setters>
                                    <Setter Property="BackgroundColor" Value="#F1F5F9" />
                                    <Setter Property="Scale" Value="1.02" />
                                    <Setter Property="Stroke" Value="#0D9488" />
                                </VisualState.Setters>
                            </VisualState>
                        </VisualStateGroup>
                    </VisualStateGroupList>
                </Setter>
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
'@

# Pages that need hover styles added (no existing ResourceDictionary)
$pagesNoResources = @(
    "Views\Sales\SalesCustomersPage.xaml",
    "Views\Finance\MonitorRevenuePage.xaml",
    "Views\Finance\FinanceDashboardPage.xaml",
    "Views\Finance\BusinessReportsPage.xaml",
    "Views\Admin\ViewSalesPage.xaml",
    "Views\Admin\ManageInventoryPage.xaml",
    "Views\Admin\CustomOrderManagementPage.xaml"
)

foreach ($page in $pagesNoResources) {
    $path = Join-Path "c:\IT13_CreatiSphere" $page
    if (Test-Path $path) {
        $content = Get-Content $path -Raw
        # Insert after BackgroundColor="..." line (closing >)
        # Find the closing > of ContentPage tag
        if ($content -match '(?s)(Shell\.NavBarIsVisible="False"\s*\r?\n\s*BackgroundColor="[^"]*">)') {
            $match = $Matches[1]
            $content = $content.Replace($match, "$match`r`n$hoverStyle")
            Set-Content $path $content -NoNewline
            Write-Host "Added resources to $page"
        } elseif ($content -match '(?s)(BackgroundColor="[^"]*">\s*\r?\n)') {
            $match = $Matches[1]
            $content = $content.Replace($match, "$match`r`n$hoverStyle`r`n")
            Set-Content $path $content -NoNewline
            Write-Host "Added resources (alt) to $page"
        } else {
            Write-Host "Could not find insertion point in $page"
        }
    }
}

Write-Host "`nDone adding ResourceDictionary sections."
