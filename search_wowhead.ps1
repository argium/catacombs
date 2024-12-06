# golden muffin
# for ($i = 228950; $i -lt 229000; $i++) {
    # $r =Invoke-WebRequest -UseBasicParsing -Uri "https://www.wowhead.com/item=$i" -MaximumRedirection 0 -ErrorAction Ignore -SkipHttpErrorCheck `

# npc: jeremy    
# for ($i = 232038; $i -lt 232048; $i++) {
    # $r =Invoke-WebRequest -UseBasicParsing -Uri "https://www.wowhead.com/npc=$i" -MaximumRedirection 0 -ErrorAction Ignore -SkipHttpErrorCheck `

# for ($i = 232038; $i -lt 232058; $i++) {
#     $r =Invoke-WebRequest -UseBasicParsing -Uri "https://www.wowhead.com/item=$i" -MaximumRedirection 0 -ErrorAction Ignore -SkipHttpErrorCheck `
for ($i = 84567; $i -lt 84607; $i++) {
    $r =Invoke-WebRequest -UseBasicParsing -Uri "https://www.wowhead.com/quest=$i" -MaximumRedirection 0 -ErrorAction Ignore -SkipHttpErrorCheck `
    -WebSession $session `
    -Headers @{
    "authority"="www.wowhead.com"
    "method"="GET"
    "path"="/item=22899811"
    "accept"="text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
    "accept-encoding"="gzip, deflate, br, zstd"
    "accept-language"="en-GB,en;q=0.9,en-US;q=0.8,en-AU;q=0.7"
    }

    if (-not $r.Headers.Location.StartsWith("https")) {
        write-host "https://www.wowhead.com" -NoNewline
    }
    write-host $r.Headers.Location
}
