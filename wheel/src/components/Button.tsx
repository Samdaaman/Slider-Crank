import React from 'react';
import {sendCommand} from "../modules/communcation";

interface Props {
    extraOnClick?: () => void;
    disabled?: boolean;
    commandKey: string;
    sendCommands?: boolean;
    title: string;
}

function Button(props: Props): JSX.Element {
    return <button
        onClick={async () => {
            console.log('Button clicked')
            if (props.sendCommands ?? true)
                await sendCommand({
                    location: props.commandKey,
                    value: 1
                });
            props.extraOnClick?.();
        }}
        title={props.title}
        disabled={props.disabled}
    >
        {props.title}
    </button>


}

export default Button;