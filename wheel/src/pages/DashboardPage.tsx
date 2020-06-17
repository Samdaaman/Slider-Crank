import React from "react";
import Switch from "../components/Switch";
import Button from "../components/Button";
import Deck from "../components/Deck";

function DashboardPage(): JSX.Element {

    return <div>
        <Switch
            commandKey={"power"}
        />
        <Button
            commandKey={"ss-up"}
            title={'Up'}
        />
        <Button
            commandKey={"ss-down"}
            title={'Down'}
        />
        <div style={{clear: "both"}}>
            <Deck deckKey={"a"}/>
        </div>
        <div style={{clear: "both"}}>
            <Deck deckKey={"b"}/>
        </div>
    </div>
}

export default DashboardPage;