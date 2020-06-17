import React from 'react';
import Button from "./Button";

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
    </div>


}

export default Deck;