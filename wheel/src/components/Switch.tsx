import React, {useState} from 'react';
import ReactSwitch from 'react-switch';
import {sendCommand} from "../modules/communcation";

interface Props {
    onSwitchChange?: (value: boolean) => void;
    disabled?: boolean;
    key: string;
    sendCommands?: boolean;
}

function Switch(props: Props) {
    const [checked, setChecked] = useState(false);
    return <ReactSwitch
        checked={checked}
        onChange={(value) => {
            setChecked(value);
            if (props.sendCommands ?? true)
                sendCommand({
                    name: props.key,
                    data: checked ? 1 : 0
                });
            props.onSwitchChange?.(value);
        }}
        disabled={props.disabled}
    />


}

export default Switch;