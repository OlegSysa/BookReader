import { NavLink, useParams, } from "react-router-dom";
import PageContent from "../PageContent";
import { useState } from "react";


export default function ContentContainer() {
    const { bookId } = useParams();
    const [loading, setLoading] = useState(false);
    if (!bookId) {
        return <div>Book not found</div>;
    }

    return (
        <div className="content-container">
            <nav>
                <NavLink
                    to="/dashboard"
                    end
                    className={({ isActive }) =>
                        `sidebar-item ${isActive ? "active" : ""}`
                    }
                >
                    ← Back
                </NavLink>
            </nav>

            <PageContent
                bookId={Number(bookId)}
                onLoadingChange={setLoading}
            />

            {loading && (
                <div className="loading-overlay">
                    <div className="spinner"></div>
                </div>
            )}
        </div>
    );
}