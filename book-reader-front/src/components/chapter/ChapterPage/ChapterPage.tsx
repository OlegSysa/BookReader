import { NavLink, useParams } from "react-router-dom";
import Chapter from "../chapter";


export default function ChapterPage() {
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
        <Chapter bookId={Number(bookId)} />;
    </div>)
}