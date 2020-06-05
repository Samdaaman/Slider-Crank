import React, {useState} from "react";
import Control from "../components/Control";

function DashboardPage(): JSX.Element {
    const [on, setOn] = useState();

    function onSwitchChange(value: boolean) {
        if (value) {
            setOn(value);
        }
        console.log(`On state changed to: ${value}`)
    }

    return <div>
        <Control
            type={'switch'}
            onSwitchChange={onSwitchChange}
            disabled={on}
        />
    </div>
}

export default DashboardPage;