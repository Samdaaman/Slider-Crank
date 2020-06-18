import React, {useState} from 'react';
import { Knob as ReactRotaryKnob } from 'react-rotary-knob';
import {sendCommand} from "../modules/communcation";

interface Props {
    extraOnChange?: (value: number) => void;
    commandKey: string;
    sendCommands?: boolean;
    knobType: "seek" | "volume";
}

// https://github.com/hugozap/react-rotary-knob#api
function Knob(props: Props): JSX.Element {
    const [position, setPosition] = useState(props.knobType == "volume" ? 50 : 0);
    const max = props.knobType == "volume" ? 100 : 1000;
    const clampMax = props.knobType == "volume" ? 270 : 360;
    const rotateDegrees = props.knobType == "volume" ? -clampMax / 2 : 0;
    return <div style={{float: "left"}}><ReactRotaryKnob
        value={position}
        unlockDistance={0}
        max={max}
        clampMax={clampMax}
        rotateDegrees={rotateDegrees}
        onEnd={async() => {
            if (props.knobType == "seek") {
                await sendCommand({
                    location: props.commandKey,
                    value: -1
                });
                setPosition(0);
            }
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
    /></div>


}

export default Knob;