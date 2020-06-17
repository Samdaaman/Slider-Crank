import React, {useState} from 'react';
import { Knob as ReactRotaryKnob } from 'react-rotary-knob';
import {sendCommand} from "../modules/communcation";

interface Props {
    extraOnChange?: (value: number) => void;
    commandKey: string;
    sendCommands?: boolean;
}

// https://github.com/hugozap/react-rotary-knob#api
function Knob(props: Props): JSX.Element {
    const [position, setPosition] = useState(0);
    return <ReactRotaryKnob
        value={position}
        unlockDistance={0}
        max={1000}
        onEnd={async() => {
            await sendCommand({
                location: props.commandKey,
                value: -1
            });
            setPosition(0);
        }}
        onChange={async(value: number) => {
            setPosition(value);
            if (props.sendCommands ?? true) {
                await sendCommand({
                    location: props.commandKey,
                    value: Math.round(value)
                });
            }
            props.extraOnChange?.(value);
        }}
    />


}

export default Knob;