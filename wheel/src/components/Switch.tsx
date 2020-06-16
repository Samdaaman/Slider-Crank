import React, {useState} from 'react';
import ReactSwitch from 'react-switch';
import {sendCommand} from "../modules/communcation";

interface Props {
    extraOnChange?: (value: boolean) => void;
    disabled?: boolean;
    commandKey: string;
    sendCommands?: boolean;
}

function Switch(props: Props): JSX.Element {
    const [checked, setChecked] = useState(false);
    return <ReactSwitch
        checked={checked}
        onChange={(value) => {
            setChecked(value);
            if (props.sendCommands ?? true)
                sendCommand({
                    location: props.commandKey,
                    value: checked ? 1 : 0
                });
            props.extraOnChange?.(value);
        }}
        disabled={props.disabled}
    />


}

export default Switch;