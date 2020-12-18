import React from "react";
import { Link, Route, Switch } from "react-router-dom";
import APP202012181226265416 from './pages/APP202012181226265416'
 //import
function App() {
    return (
        <main>
            <Switch>
                <Route path='/APP202012181226265416' component={APP202012181226265416} />
 {/* Route */}
            </Switch>
        </main>
    )
}

export default App;