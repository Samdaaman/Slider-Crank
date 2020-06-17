import React from 'react';
import Button from "./Button";
import Knob from "./Knob";

interface Props {
    deckKey: string;
}

function Deck(props: Props): JSX.Element {
    return <div>
        <Button
            commandKey={`d-${props.deckKey}-sselect`}
            title={`${props.deckKey}-select`}
        />
        <Button
            commandKey={`d-${props.deckKey}-splay`}
            title={`${props.deckKey}-Play`}
        />
        <Button
            commandKey={`d-${props.deckKey}-spause`}
            title={`${props.deckKey}-Pause`}
        />
        <Button
            commandKey={`d-${props.deckKey}-sstop`}
            title={`${props.deckKey}-Stop`}
        />
        <Knob
            commandKey={`d-${props.deckKey}-seekrb`}
        />
        <Knob
            commandKey={`d-${props.deckKey}-seekrf`}
        />
    </div>


}

export default Deck;