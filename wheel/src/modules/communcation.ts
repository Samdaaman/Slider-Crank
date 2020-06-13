const socket = new WebSocket("tcp://localhost:727");

interface Command {
    name: string;
    data: number;
}

export function sendCommand(command: Command): void {
    socket.send(JSON.stringify(command));
}
