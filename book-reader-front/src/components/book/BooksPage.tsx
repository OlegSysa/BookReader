import { useEffect, useState } from "react";
import { getAllUserBooks } from "../../api/Book";
import type { BookModel } from "../../api/models/book";
import "./BooksPage.css"
import AddBookModal from "./AddBookModal"
import { useNavigate } from "react-router-dom";

export default function BooksPage() {
    const [books, setBooks] = useState<BookModel[]>([]);
    const [isAddBookModalOpen, setIsAddBookModalOpen] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        getAllUserBooks()
            .then(response => {
                if (response.success) {
                    setBooks(response.data);
                }
            })
            .catch(error => {
                console.error(error);
            });
    }, []);
    return (
        <div className="books-page">
            <div className="books-page-header">
                <h1>Books Page</h1>

                <button
                    className="add-book-button"
                    onClick={() => setIsAddBookModalOpen(true)}
                >
                    + Add book
                </button>
            </div>

            <AddBookModal
                isOpen={isAddBookModalOpen}
                onClose={() => setIsAddBookModalOpen(false)}
            />

            <div className="books-grid">
                {books.map(book => (
                    <div
                        className="book-card"
                        key={book.id}
                        onClick={() => navigate(`/books/${book.id}`)}
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

                                <span>{book.status}</span>
                            </div>

                            <div className="book-card-date">
                                {new Date(book.createdAtUtc).toLocaleDateString()}
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}