import React from "react";
import Switch from "../components/Switch";
import Button from "../components/Button";

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
        <Button
            commandKey={"ss-select"}
            title={'Select'}
        />
    </div>
}

export default DashboardPage;