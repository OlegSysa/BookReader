import { NavLink, useNavigate } from "react-router-dom";
import "./Dashboard.css";
import { Logout } from "../../api/Auth";

export default function Sidebar() {

    const navigate = useNavigate();
    const handleLogout = async () => {
        await Logout();
        navigate("/");
    };

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

                <button onClick={handleLogout} className="sidebar-item">
                    Logout
                </button>
            </nav>
        </aside>
    );
}