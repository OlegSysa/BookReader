import { NavLink } from "react-router-dom";
import "./Dashboard.css";

export default function Sidebar() {
    return (
        <aside className="dashboard-sidebar">
            <nav>
                <NavLink
                    to="/dashboard"
                    end
                    className={({ isActive }) =>
                        `sidebar-item ${isActive ? "active" : ""}`
                    }
                >
                    Books
                </NavLink>

                <NavLink
                    to="/dashboard/profile"
                    className={({ isActive }) =>
                        `sidebar-item ${isActive ? "active" : ""}`
                    }
                >
                    Profile
                </NavLink>

                <NavLink
                    to="/dashboard/words"
                    className={({ isActive }) =>
                        `sidebar-item ${isActive ? "active" : ""}`
                    }
                >
                    Words
                </NavLink>

                <button className="sidebar-item">
                    Logout
                </button>
            </nav>
        </aside>
    );
}