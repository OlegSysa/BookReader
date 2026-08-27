import { NavLink, useParams } from "react-router-dom";
import PageContent from "../PageContent";


export default function ContentContainer() {
    const { bookId } = useParams();

    if (!bookId) {
        return <div>Book not found</div>;
    }

    return (<div>
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
        <PageContent bookId={Number(bookId)} />;
    </div>)
}