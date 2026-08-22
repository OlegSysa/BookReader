import { useEffect, useState } from "react";
import { getAllUserBooks } from "../../api/ApiClient";
import type { BookModel } from "../../api/models/book";
import "./BooksPage.css"
import AddBookModal from "./AddBookModal"
import { useOutletContext } from "react-router-dom";
import BookCard from "./BookCard";

export default function BooksPage() {
    const [books, setBooks] = useState<BookModel[]>([]);
    const [isAddBookModalOpen, setIsAddBookModalOpen] = useState(false);
    const { bookStatuses } = useOutletContext<{
        bookStatuses: Record<number, number>;
    }>();
    const loadBooks = async () => {
        try {
            const response = await getAllUserBooks();

            if (response.success) {
                setBooks(response.data);
            }
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        loadBooks();
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
                onBookAdded={loadBooks}
            />

            <div className="books-grid">
                {books.map(book => (
                    <BookCard
                        key={book.id}
                        book={book}
                        status={bookStatuses[book.id] ?? book.status}
                    />
                ))}
            </div>
        </div>
    );
}