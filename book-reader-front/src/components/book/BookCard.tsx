import { useNavigate } from "react-router-dom";
import type { BookModel } from "../../api/models/book";
import BookStatus from "./BookStatus";
import "./BookCard.css"
import deleteIcon from "../../assets/delete_book.png"
import { deleteBook } from "../../api/ApiClient";

interface BookCardProps {
    book: BookModel;
    status: number;
    onBooksChanged: () => void;
}

export default function BookCard({ book, status, onBooksChanged }: BookCardProps) {
    const navigate = useNavigate();

    const delete_Book = async (e: React.MouseEvent<HTMLButtonElement>) => {
        e.stopPropagation();
        try {
            const response = await deleteBook(book.id);

            if (response.success) {
                onBooksChanged();
            }
        } catch (error) {
            console.error(error);
        }
    };

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
                    <span>
                        <button onClick={delete_Book} className="delete-book-button">
                            <img src={deleteIcon} alt="Delete" />
                        </button>
                    </span>
                </div>

                <div className="book-card-date">
                    {new Date(book.createdAtUtc).toLocaleDateString()}
                </div>
            </div>
        </div>
    );
};