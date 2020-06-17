declare module 'react-rotary-knob' {
    /**
     * Show the rotation circle and marker
     * dispatches drag events
     */

    import { Component } from "react";
    import type { Point } from "./Types";

    /**
     * type definition for the skin system attribute modification element
     */
    type AttributeSetValue = {
        name: string,
        value: (props: any, value: any) => string
    };

    /**
     * Type definition for the skin element manipulation block
     */
    type UpdateElement = {
        element: string,
        content?: (props: any, value: any) => string,
        attrs: Array<AttributeSetValue>
    };
    /**
     * A skin consists of the svg code
     * and the knob element centerx and y (knobX, knobY)
     */
    type Skin = {
        svg: string,
        knobX: number,
        knobY: number,
        updateAttributes: Array<UpdateElement>
    };

    type KnobProps = {
        value?: number,
        defaultValue?: number,
        clampMin?: number,
        clampMax?: number,
        rotateDegrees?: number,
        min?: number,
        max?: number,
        skin?: Skin,
        format?: (val: number) => string,
        onChange?: (val: number) => void,
        onStart?: () => void,
        onEnd?: () => void,
        style?: any,
        preciseMode?: boolean,
        unlockDistance?: number,
        step?: number
    };

    type KnobState = {
        svgRef: any,
        dragging: boolean,
        dragDistance: number,
        mousePos: Point,
        valueAngle: number,
        uncontrolledValue?: number
    };

    /**
     * Generic knob component
     */
    class Knob extends Component<KnobProps, KnobState> {
        container: any;
        scale: any;
        scaleProps: {min: number, max: number, clampMin: number, clampMax: number}
        inputRef: any;
        controlled: boolean;

        state = {
            svgRef: null,
            dragging: false,
            dragDistance: 0,
            mousePos: { x: 0, y: 0 },
            valueAngle: 0,
            uncontrolledValue: 0
        };



        render: () => any
    }

    export { Knob };

}