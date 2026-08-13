import { Outlet } from "react-router-dom";
import "./Dashboard.css";
import Sidebar from "./Sidebar";

export default function DashboardPage() {
    return (
        <main className="dashboard-page">
            <div className="dashboard-container">
                <Sidebar />

                <section className="dashboard-content">
                    <Outlet />
                </section>
            </div>
        </main>
    );
}