import axios from 'axios'

const HOST = 'http://192.168.1.2:727/socket';

interface Command {
    location: string;
    value: number;
}

export async function sendCommand(command: Command): Promise<void> {
    try {
        await axios.post(HOST, `${command.location}:${command.value};\n`);
    }
    catch (error) {
        // alert(`Yeet2: ${error.toString()}`);
    }
}
