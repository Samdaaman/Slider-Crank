declare module 'react-rotary-knob' {
    import React from "react";
    interface Props {
        // default=0 Minimum value
        min?: number;
        // default=100 Maximum value
        max?: number;
        // default=0 Control Value
        value?: number;
        // default=0 start value for uncontrolled mode
        defaultValue?: number;
        // Callback with the updated value
        onChange?: (value: number) => void;
        // default=100 Minimum drag distance required to unlock the knob
        unlockDistance?: number;
        // default=1 the step distance (when using the keyboard arrows)
        step?: number;
        // Skin object
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        skin?: any;
        // Called when the dragging starts
        onStart?: () => void;
        // Called when the dragging ends
        onEnd?: () => void;
        // default=0 degree value to move the starting point of the active area of the knob away from the center
        clampMin?: number;
        // default=360 degree value to move the end point of the active area of the knob away from the center
        clampMax?: number;
        // default=0 (zero is at top) degree value to rotate the knob component to have the starting / end points at a different position
        rotateDegress?: number;
    }
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    class ReactRotaryKnob extends React.Component<Props, any> {

    }
    export = ReactRotaryKnob;
}