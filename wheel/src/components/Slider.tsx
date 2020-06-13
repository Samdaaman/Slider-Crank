import React, { useState } from "react";
import Switch from "react-switch";

interface Props {
  type: "switch";
  onSwitchChange: (value: boolean) => void;
  disabled?: boolean;
}

function Slider(props: Props) {
    switch (props.type) {
    case "switch": {
        const [checked, setChecked] = useState(false);
        return (
            <Switch
                checked={checked}
                onChange={(value) => {
                    setChecked(value);
            props.onSwitchChange?.(value);
                }}
                disabled={props.disabled}
            />
        );
    }
    default: {
        throw new Error("Missing break statement");
    }
    }
}

export default Slider;
