import { useEffect, useState } from "react";
import { getAllUserBooks } from "../../api/Book";
import type { BookModel } from "../../api/models/book";
import "./BooksPage.css"
export default function BooksPage() {
    const [books, setBooks] = useState<BookModel[]>([]);

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
        <div>
            <h1>Books Page</h1>

            <div className="books-grid">
                {books.map(book => (
                    <div className="book-card" key={book.id}>
                        <div className="book-card-icon">
                            📖
                        </div>

                        <div className="book-card-content">
                            <h2>{book.originalFileName}</h2>

                            <div className="book-card-info">
                                <span>{(book.fileSize / 1024 / 1024).toFixed(2)} MB</span>
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