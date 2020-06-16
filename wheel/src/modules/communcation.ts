import axios from 'axios'

const HOST = 'http://localhost:727/socket';

interface Command {
    location: string;
    value: number;
}

export async function sendCommand(command: Command): Promise<void> {
    await axios.post(HOST, [{
        location: command.location,
        value: command.value
    }]);
}
