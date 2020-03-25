# swvo-websocket-terminator
Command-line tool that terminates a SonicWall Virtual Office bookmark websocket as a local port.

To use to terminate an SSH bookmark as local port 2222:

```swvo-ws-terminator 44E4224D3E4B1C74FCADE675803C1162 "wss://sonicwall.company.com:4433/ws?bookmark=SSH" 2222```

The first argument is the value if the SessId cookie from your authenticated web browser.
