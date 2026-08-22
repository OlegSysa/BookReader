import { useNavigate } from "react-router-dom";
import type { BookModel } from "../../api/models/book";
import BookStatus from "./BookStatus";
import "./BookCard.css"

interface BookCardProps {
    book: BookModel;
    status: number;
}

export default function BookCard({ book, status }: BookCardProps) {
    const navigate = useNavigate();

    return (
        <div
            className={`book-card ${status !== 4 ? "book-card-disabled" : ""}`}
            key={book.id}
            onClick={() => {
                if (status === 4) {
                    navigate(`/books/${book.id}`);
                }
            }}
        >
            <div className="book-card-icon">
                📖
            </div>

            <div className="book-card-content">
                <h2>{book.originalFileName}</h2>

                <div className="book-card-info">
                    <span>
                        {(book.fileSize / 1024 / 1024).toFixed(2)} MB
                    </span>

                    <BookStatus status={status} />
                </div>

                <div className="book-card-date">
                    {new Date(book.createdAtUtc).toLocaleDateString()}
                </div>
            </div>
        </div>
    );
};