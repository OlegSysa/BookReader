import { Outlet } from "react-router-dom";
import "./Dashboard.css";
import Sidebar from "./Sidebar";
import { useNotifications } from "../../hooks/useNotifications";

export default function DashboardPage() {
    const bookStatuses = useNotifications();
    return (
        <main className="dashboard-page">
            <div className="dashboard-container">
                <Sidebar />

                <section className="dashboard-content">
                    <Outlet context={{ bookStatuses }} />
                </section>
            </div>
        </main>
    );
}