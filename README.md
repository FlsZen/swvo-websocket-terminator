# swvo-websocket-terminator
Command-line tool that terminates a SonicWall Virtual Office bookmark websocket as a local port.

To use to terminate a bookmark named "SSH" as local port 2222:

`swvo-ws-terminator 44E4224D3E4B1C74FCADE675803C1162 sonicwall.company.com:4433 SSH 2222`

The first argument is the value of the `SessId` cookie from your authenticated web browser.
