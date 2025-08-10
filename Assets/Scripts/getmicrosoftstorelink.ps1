# Credit: ThioJoe
# License: MIT License
# Source: https://github.com/ThioJoe/Windows-Sandbox-Tools/blob/main/Installer%20Scripts/Install-Microsoft-Store.ps1
# Changes: Modified to get any Microsoft Store Package or its dependencies and output only the download urls.

param(
    [string]$AppxPackageFamilyName,
    [string]$CategoryId,
    [string]$Filetype,
    [int]$PackageIndex = 0,
    [switch]$Dependencies
)

$flightRing = "Retail"
$flightingBranchName = ""
$currentBranch = "ge_release"

# --- XML Templates ---

# Step 1: GetCookie request body.
# See: https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wusp/36a5d99a-a3ca-439d-bcc5-7325ff6b91e2
$cookieXmlTemplate = @"
<s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:a="http://www.w3.org/2005/08/addressing">
    <s:Header>
        <a:Action s:mustUnderstand="1">http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetCookie</a:Action>
        <a:MessageID>urn:uuid:$(New-Guid)</a:MessageID>
        <a:To s:mustUnderstand="1">https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx</a:To>
        <o:Security s:mustUnderstand="1" xmlns:o="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <wuws:WindowsUpdateTicketsToken wsu:id="ClientMSA" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" xmlns:wuws="http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization">
                <TicketType Name="MSA" Version="1.0" Policy="MBI_SSL"><user></user></TicketType>
            </wuws:WindowsUpdateTicketsToken>
        </o:Security>
    </s:Header>
    <s:Body><GetCookie xmlns="http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService" /></s:Body>
</s:Envelope>
"@

# Step 2: SyncUpdates request body. Based on intercepted XML request using Microsoft Store.
# Info about attributes found here: https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wusp/6b654980-ae63-4b0d-9fae-2abb516af894
$fileListXmlTemplate = @"
<s:Envelope xmlns:a="http://www.w3.org/2005/08/addressing" xmlns:s="http://www.w3.org/2003/05/soap-envelope">
    <s:Header>
        <a:Action s:mustUnderstand="1">http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/SyncUpdates</a:Action>
        <a:MessageID>urn:uuid:$(New-Guid)</a:MessageID>
        <a:To s:mustUnderstand="1">https://fe3cr.delivery.mp.microsoft.com/ClientWebService/client.asmx</a:To>
        <o:Security s:mustUnderstand="1" xmlns:o="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <Timestamp xmlns="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
                <Created>$((Get-Date).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"))</Created>
                <Expires>$((Get-Date).AddMinutes(5).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"))</Expires>
            </Timestamp>
            <wuws:WindowsUpdateTicketsToken wsu:id="ClientMSA" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" xmlns:wuws="http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization">
                <TicketType Name="MSA" Version="1.0" Policy="MBI_SSL">
                    <user/>
                </TicketType>
            </wuws:WindowsUpdateTicketsToken>
        </o:Security>
    </s:Header>
    <s:Body>
        <SyncUpdates xmlns="http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService">
            <cookie>
                <Expiration>$((Get-Date).AddYears(10).ToUniversalTime().ToString('u').Replace(' ','T'))</Expiration>
                <EncryptedData>{0}</EncryptedData>
            </cookie>
            <parameters>
                <ExpressQuery>false</ExpressQuery>
                <InstalledNonLeafUpdateIDs>
                    <int>1</int><int>2</int><int>3</int><int>11</int><int>19</int><int>2359974</int><int>5169044</int>
                    <int>8788830</int><int>23110993</int><int>23110994</int><int>54341900</int><int>59830006</int><int>59830007</int>
                    <int>59830008</int><int>60484010</int><int>62450018</int><int>62450019</int><int>62450020</int><int>98959022</int>
                    <int>98959023</int><int>98959024</int><int>98959025</int><int>98959026</int><int>104433538</int><int>129905029</int>
                    <int>130040031</int><int>132387090</int><int>132393049</int><int>133399034</int><int>138537048</int><int>140377312</int>
                    <int>143747671</int><int>158941041</int><int>158941042</int><int>158941043</int><int>158941044</int><int>159123858</int>
                    <int>159130928</int><int>164836897</int><int>164847386</int><int>164848327</int><int>164852241</int><int>164852246</int>
                    <int>164852253</int>
                </InstalledNonLeafUpdateIDs>
                <SkipSoftwareSync>false</SkipSoftwareSync>
                <NeedTwoGroupOutOfScopeUpdates>false</NeedTwoGroupOutOfScopeUpdates>
                <FilterAppCategoryIds>
                    <CategoryIdentifier>
                        <Id>{1}</Id>
                    </CategoryIdentifier>
                </FilterAppCategoryIds>
                <TreatAppCategoryIdsAsInstalled>true</TreatAppCategoryIdsAsInstalled>
                <AlsoPerformRegularSync>false</AlsoPerformRegularSync>
                <ComputerSpec/>
                <ExtendedUpdateInfoParameters>
                    <XmlUpdateFragmentTypes>
                        <XmlUpdateFragmentType>Extended</XmlUpdateFragmentType>
                    </XmlUpdateFragmentTypes>
                    <Locales>
                        <string>en-US</string>
                        <string>en</string>
                    </Locales>
                </ExtendedUpdateInfoParameters>
                <ClientPreferredLanguages>
                    <string>en-US</string>
                </ClientPreferredLanguages>
                <ProductsParameters>
                    <SyncCurrentVersionOnly>false</SyncCurrentVersionOnly>
                    <DeviceAttributes>E:BranchReadinessLevel=CB&amp;CurrentBranch={2}&amp;OEMModel=Virtual%20Machine&amp;FlightRing={3}&amp;AttrDataVer=321&amp;InstallLanguage=en-US&amp;OSUILocale=en-US&amp;InstallationType=Client&amp;FlightingBranchName={4}&amp;OSSkuId=48&amp;App=WU_STORE&amp;ProcessorManufacturer=GenuineIntel&amp;OEMName_Uncleaned=Microsoft%20Corporation&amp;AppVer=1407.2503.28012.0&amp;OSArchitecture=AMD64&amp;IsFlightingEnabled=1&amp;TelemetryLevel=1&amp;DefaultUserRegion=39070&amp;WuClientVer=1310.2503.26012.0&amp;OSVersion=10.0.26100.3915&amp;DeviceFamily=Windows.Desktop</DeviceAttributes>
                    <CallerAttributes>Interactive=1;IsSeeker=1;</CallerAttributes>
                    <Products/>
                </ProductsParameters>
            </parameters>
        </SyncUpdates>
    </s:Body>
</s:Envelope>
"@

# Step 3: GetExtendedUpdateInfo2 - After getting the list of matched files (app version and dependencies), this lets us get the actual download URLs
# See: https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wusp/2f66a682-164f-47ec-968e-e43c0a85dc21
$fileUrlXmlTemplate = @"
<s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:a="http://www.w3.org/2005/08/addressing">
    <s:Header>
        <a:Action s:mustUnderstand="1">http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetExtendedUpdateInfo2</a:Action>
        <a:MessageID>urn:uuid:$(New-Guid)</a:MessageID>
        <a:To s:mustUnderstand="1">https://fe3cr.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured</a:To>
        <o:Security s:mustUnderstand="1" xmlns:o="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <u:Timestamp u:Id="_0" xmlns:u="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
                <u:Created>$((Get-Date).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"))</u:Created>
                <u:Expires>$((Get-Date).AddMinutes(5).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"))</u:Expires>
            </u:Timestamp>
            <wuws:WindowsUpdateTicketsToken wsu:id="ClientMSA" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" xmlns:wuws="http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization">
                <TicketType Name="MSA" Version="1.0" Policy="MBI_SSL"><user>{0}</user></TicketType>
            </wuws:WindowsUpdateTicketsToken>
        </o:Security>
    </s:Header>
    <s:Body>
        <GetExtendedUpdateInfo2 xmlns="http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService">
            <updateIDs><UpdateIdentity><UpdateID>{1}</UpdateID><RevisionNumber>{2}</RevisionNumber></UpdateIdentity></updateIDs>
            <infoTypes><XmlUpdateFragmentType>FileUrl</XmlUpdateFragmentType></infoTypes>
            <DeviceAttributes>E:BranchReadinessLevel=CB&amp;CurrentBranch={3}&amp;OEMModel=Virtual%20Machine&amp;FlightRing={4}&amp;AttrDataVer=321&amp;InstallLanguage=en-US&amp;OSUILocale=en-US&amp;InstallationType=Client&amp;FlightingBranchName={5}&amp;OSSkuId=48&amp;App=WU_STORE&amp;ProcessorManufacturer=GenuineIntel&amp;OEMName_Uncleaned=Microsoft%20Corporation&amp;AppVer=1407.2503.28012.0&amp;OSArchitecture=AMD64&amp;IsFlightingEnabled=1&amp;TelemetryLevel=1&amp;DefaultUserRegion=39070&amp;WuClientVer=1310.2503.26012.0&amp;OSVersion=10.0.26100.3915&amp;DeviceFamily=Windows.Desktop</DeviceAttributes>
        </GetExtendedUpdateInfo2>
    </s:Body>
</s:Envelope>
"@

# --- Script Execution ---
$headers = @{ "Content-Type" = "application/soap+xml; charset=utf-8" }
$baseUri = "https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx"

# Step 1: Get Cookie
$cookieRequestPayload = $cookieXmlTemplate
If ($debugSaveFiles) { $cookieRequestPayload | Set-Content -Path (Join-Path $LogDirectory "01_Step1_Request.xml") }

$cookieResponse = Invoke-WebRequest -Uri $baseUri -Method Post -Body $cookieRequestPayload -Headers $headers -UseBasicParsing
If ($debugSaveFiles) { $cookieResponse.Content | Set-Content -Path (Join-Path $LogDirectory "01_Step1_Response.xml"); Write-Host "  -> Saved request and response logs for Step 1." }

$cookieResponseXml = [xml]$cookieResponse.Content
$encryptedCookieData = $cookieResponseXml.Envelope.Body.GetCookieResponse.GetCookieResult.EncryptedData

# Step 2: Get File List
$fileListRequestPayload = $fileListXmlTemplate -f $encryptedCookieData, $CategoryId, $currentBranch, $flightRing, $flightingBranchName
If ($debugSaveFiles) { [System.IO.File]::WriteAllText((Join-Path $LogDirectory "02_Step2_Request_AUTOMATED.xml"), $fileListRequestPayload, [System.Text.UTF8Encoding]::new($false)) }

$fileListResponse = Invoke-WebRequest -Uri $baseUri -Method Post -Body $fileListRequestPayload -Headers $headers -UseBasicParsing
If ($debugSaveFiles) { $fileListResponse.Content | Set-Content -Path (Join-Path $LogDirectory "02_Step2_Response_SUCCESS.xml") }

# The response contains XML fragments that are HTML-encoded. We must decode this before treating it as XML.
Add-Type -AssemblyName System.Web
$decodedContent = [System.Web.HttpUtility]::HtmlDecode($fileListResponse.Content)
$fileListResponseXml = [xml]$decodedContent

$fileIdentityMap = @{}

# Get the two main lists of updates from the now correctly-decoded response
$newUpdates = $fileListResponseXml.Envelope.Body.SyncUpdatesResponse.SyncUpdatesResult.NewUpdates.UpdateInfo
$allExtendedUpdates = $fileListResponseXml.Envelope.Body.SyncUpdatesResponse.SyncUpdatesResult.ExtendedUpdateInfo.Updates.Update

# Filter the 'NewUpdates' list to only include items that are actual downloadable files.
# These are identified by the presence of the <SecuredFragment> tag inside their inner XML.
$downloadableUpdates = $newUpdates | Where-Object { $_.Xml.Properties.SecuredFragment }

# Now, process each downloadable update
foreach ($update in $downloadableUpdates) {
    $lookupId = $update.ID
    
    # Find the matching entry in the 'ExtendedUpdateInfo' list using the same numeric ID.
    $extendedInfo = $allExtendedUpdates | Where-Object { $_.ID -eq $lookupId } | Select-Object -First 1
        
    # From the extended info, get the actual package file and ignore the metadata .cab files.
    $fileNode = $extendedInfo.Xml.Files.File | Where-Object { $_.FileName -and $_.FileName -notlike "Abm_*" } | Select-Object -First 1

    # Additional parsing
    $fileName = $fileNode.FileName
    $updateGuid = $update.Xml.UpdateIdentity.UpdateID
    $revNum = $update.Xml.UpdateIdentity.RevisionNumber
    $fullIdentifier = $fileNode.GetAttribute("InstallerSpecificIdentifier")

    # Define the regex based on the official package identity structure.
    # <Name>_<Version>_<Architecture>_<ResourceId>_<PublisherId>
    $regex = "^(?<Name>.+?)_(?<Version>\d+\.\d+\.\d+\.\d+)_(?<Architecture>[a-zA-Z0-9]+)_(?<ResourceId>.*?)_(?<PublisherId>[a-hjkmnp-tv-z0-9]{13})$"
    
    $packageInfo = [PSCustomObject]@{
        FullName       = $fullIdentifier
        FileName       = $fileName
        UpdateID       = $updateGuid
        RevisionNumber = $revNum
    }

    if ($fullIdentifier -match $regex) {
        # If the regex matches, populate the object with the named capture groups
        $packageInfo | Add-Member -MemberType NoteProperty -Name "PackageName" -Value $matches.Name
        $packageInfo | Add-Member -MemberType NoteProperty -Name "Version" -Value $matches.Version
        $packageInfo | Add-Member -MemberType NoteProperty -Name "Architecture" -Value $matches.Architecture
        $packageInfo | Add-Member -MemberType NoteProperty -Name "ResourceId" -Value $matches.ResourceId
        $packageInfo | Add-Member -MemberType NoteProperty -Name "PublisherId" -Value $matches.PublisherId
    } else {
        # Fallback for any identifiers that don't match the pattern
        $packageInfo | Add-Member -MemberType NoteProperty -Name "PackageName" -Value "Unknown (Parsing Failed)"
        $packageInfo | Add-Member -MemberType NoteProperty -Name "Architecture" -Value "unknown"
    }

    # Use the full, unique identifier as the key in the map
    $fileIdentityMap[$fullIdentifier] = $packageInfo
}

# Get the current system's processor architecture and map it to the script's naming convention
$systemArch = switch ($env:PROCESSOR_ARCHITECTURE) {
    "AMD64" { "x64" }
    "ARM64" { "arm64" }
    "x86"   { "x86" }
    default { "unknown" }
}

# Filter latest package
$latestPackage = $fileIdentityMap.Values |
    Where-Object { $_.PackageName -eq $AppxPackageFamilyName } |
    Where-Object { $_.FileName -like "*.$Filetype" } |
    Sort-Object { [version]$_.Version } -Descending |
    Select-Object -Index $PackageIndex

# Get all dependencies and only keep latest version of each
$latestDependencies = $fileIdentityMap.Values |
    Where-Object {
        ($_.PackageName -ne $AppxPackageFamilyName) -and
        ( ($_.Architecture -eq $systemArch) -or ($_.Architecture -eq 'neutral') )
    } |
    Group-Object {
        $_.PackageName -replace '\.\d+(\.\d+)*$', ''
    } |
    ForEach-Object {
        $_.Group | Sort-Object { [version]$_.Version } -Descending | Select-Object -First 1
    }

if (-not $Dependencies) {
    # Output only the main package
    if ($latestPackage) {
        foreach ($package in @($latestPackage)) {
            $fileUrlRequestPayload = $fileUrlXmlTemplate -f $encryptedCookieData, $package.UpdateID, $package.RevisionNumber, $currentBranch, $flightRing, $flightingBranchName
            $fileUrlResponse = Invoke-WebRequest -Uri "$baseUri/secured" -Method Post -Body $fileUrlRequestPayload -Headers $headers -UseBasicParsing
            $fileUrlResponseXml = [xml]$fileUrlResponse.Content

            $fileLocations = $fileUrlResponseXml.Envelope.Body.GetExtendedUpdateInfo2Response.GetExtendedUpdateInfo2Result.FileLocations.FileLocation
            $downloadUrl = ($fileLocations | Where-Object { $_.Url -like "http://tlu.dl.delivery.mp.microsoft.com*" }).Url

            if ($downloadUrl) {
                Write-Host "$downloadUrl" -ForegroundColor Cyan
            }
        }
    }
}
else {
    # Output only the dependencies
    if ($latestDependencies) {
        foreach ($package in $latestDependencies) {
            $fileUrlRequestPayload = $fileUrlXmlTemplate -f $encryptedCookieData, $package.UpdateID, $package.RevisionNumber, $currentBranch, $flightRing, $flightingBranchName
            $fileUrlResponse = Invoke-WebRequest -Uri "$baseUri/secured" -Method Post -Body $fileUrlRequestPayload -Headers $headers -UseBasicParsing
            $fileUrlResponseXml = [xml]$fileUrlResponse.Content

            $fileLocations = $fileUrlResponseXml.Envelope.Body.GetExtendedUpdateInfo2Response.GetExtendedUpdateInfo2Result.FileLocations.FileLocation
            $downloadUrl = ($fileLocations | Where-Object { $_.Url -like "http://tlu.dl.delivery.mp.microsoft.com*" }).Url

            if ($downloadUrl) {
                Write-Host "$downloadUrl" -ForegroundColor Cyan
            }
        }
    }
}