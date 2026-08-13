import { Outlet } from "react-router-dom";
import "./MainLayout.css";

export default function MainLayout() {
    return (
        <div className="main-layout">
            <header className="main-layout-header">
                <div className="main-layout-logo">
                    BooklyWorld
                </div>
            </header>

            <main className="main-layout-content">
                <Outlet />
            </main>
        </div>
    );
}