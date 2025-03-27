import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';

export default function MonthView() {
    const { year, month } = useParams();
    const [transactions, setTransactions] = useState([]);
    const [showForm, setShowForm] = useState(false);
    const [preferredCurrency, setPreferredCurrency] = useState('PLN');
    
    const [categories, setCategories] = useState([]);

    const [editingTransaction, setEditingTransaction] = useState(null);
    const [editForm, setEditForm] = useState({
        description: '',
        amount: '',
        date: '',
        categoryId: '',
        isIncome: false
    });

    const [form, setForm] = useState({
        description: '',
        amount: '',
        date: new Date().toISOString().split('T')[0],
        categoryId: '',
        isIncome: false
    });
    
    const monthNames = [
        "",
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    ];

    useEffect(() => {
        const fetchUserCurrency = async () => {
            try {
                const res = await fetch('http://localhost:5201/api/user/me', {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });
                const data = await res.json();
                setPreferredCurrency(data.currency || 'PLN');
            } catch (err) {
                console.error('Could not fetch user currency', err);
            }
        };

        const fetchTransactions = async () => {
            try {
                const res = await fetch(
                    `http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`,
                    {
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem('token')}`
                        }
                    }
                );
                const data = await res.json();
                setTransactions(data);
            } catch (err) {
                console.error('Failed to fetch transactions.', err);
            }
        };

        const fetchCategories = async () => {
            try {
                const res = await fetch('http://localhost:5201/api/category', {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });
                const data = await res.json();
                setCategories(data);
            } catch (err) {
                console.error('Failed to fetch categories', err);
            }
        };

        fetchUserCurrency();
        fetchTransactions();
        fetchCategories();
    }, [month, year]);
    
    const handleAdd = async (e) => {
        e.preventDefault();
        try {
            const res = await fetch('http://localhost:5201/api/transaction', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify({
                    description: form.description,
                    amount: parseFloat(form.amount),
                    date: form.date,
                    isIncome: form.isIncome,
                    categoryId: form.categoryId,
                    currency: preferredCurrency
                })
            });

            if (!res.ok) throw new Error('Failed to add transaction.');

            setForm({
                description: '',
                amount: '',
                date: new Date().toISOString().split('T')[0],
                categoryId: '',
                isIncome: false
            });
            setShowForm(false);
            
            const updatedRes = await fetch(
                `http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`,
                {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                }
            );
            const updatedData = await updatedRes.json();
            setTransactions(updatedData);
        } catch (err) {
            alert('Failed to add transaction.');
        }
    };
    const handleDelete = async (id) => {
        const confirmDelete = window.confirm(
            'Are you sure you want to delete this transaction?'
        );
        if (!confirmDelete) return;

        try {
            const res = await fetch(`http://localhost:5201/api/transaction/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!res.ok) throw new Error('Failed to delete transaction.');
            
            const updatedRes = await fetch(
                `http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`,
                {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                }
            );
            const updatedData = await updatedRes.json();
            setTransactions(updatedData);
        } catch (err) {
            alert('Failed to delete transaction.');
        }
    };
    
    const handleEditClick = (t) => {
        setEditingTransaction(t);
        setEditForm({
            description: t.description,
            amount: t.amount,
            date: t.date.split('T')[0],
            categoryId: t.categoryId,
            isIncome: t.isIncome
        });
    };
    
    const handleEditSubmit = async (e) => {
        e.preventDefault();
        if (!editingTransaction) return;

        try {
            const res = await fetch(
                `http://localhost:5201/api/transaction/${editingTransaction.id}`,
                {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    },
                    body: JSON.stringify({
                        ...editForm,
                        amount: parseFloat(editForm.amount),
                        currency: preferredCurrency
                    })
                }
            );
            if (!res.ok) throw new Error('Failed to update transaction.');

            setEditingTransaction(null);

            const updatedRes = await fetch(
                `http://localhost:5201/api/transaction/monthly?month=${month}&year=${year}`,
                {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                }
            );
            const updatedData = await updatedRes.json();
            setTransactions(updatedData);
        } catch (err) {
            alert('Update failed');
        }
    };
    
    return (
        <div id="month-view">
            <div id="header">
                <h2>
                    Transactions for {monthNames[Number(month)]} {year}
                </h2>
                <button id="add-btn" onClick={() => setShowForm(!showForm)}>
                    +
                </button>
            </div>
            
            {showForm && (
                <form id="add-transaction-form" onSubmit={handleAdd}>
                    <input
                        placeholder="Description"
                        value={form.description}
                        onChange={(e) => setForm({ ...form, description: e.target.value })}
                        required
                    />
                    <input
                        placeholder="Amount"
                        type="number"
                        step="0.01"
                        value={form.amount}
                        onChange={(e) => setForm({ ...form, amount: e.target.value })}
                        required
                    />
                    <input
                        type="date"
                        value={form.date}
                        onChange={(e) => setForm({ ...form, date: e.target.value })}
                        required
                    />
                    <select
                        value={form.categoryId}
                        onChange={(e) => setForm({ ...form, categoryId: e.target.value })}
                        required
                    >
                        <option value="">-- Select Category --</option>
                        {categories.map((c) => (
                            <option key={c.id} value={c.id}>
                                {c.name}
                            </option>
                        ))}
                    </select>
                    <label>
                        <input
                            type="checkbox"
                            checked={form.isIncome}
                            onChange={(e) =>
                                setForm({ ...form, isIncome: !form.isIncome })
                            }
                        />
                        Is Income
                    </label>
                    <button type="submit">Add</button>
                </form>
            )}
            
            <ul id="transaction-list">
                {transactions.map((t) => (
                    <li key={t.id}>
                        <div>
                            <strong>{t.description}</strong> – {t.amount} {t.currency}
                            <br />
                            <small>
                                {t.date.split('T')[0]} | {t.category?.name} |{' '}
                                {t.isIncome ? 'Income' : 'Expense'}
                            </small>
                            <div>
                                <button onClick={() => handleDelete(t.id)}>🗑️ Delete</button>
                                <button onClick={() => handleEditClick(t)}>✏️ Edit</button>
                            </div>
                        </div>
                    </li>
                ))}
            </ul>
            
            {editingTransaction && (
                <div id="edit-modal">
                    <div id="edit-modal-header">
                        <h3>Edit Transaction</h3>
                        <button
                            id="close-edit-modal"
                            onClick={() => setEditingTransaction(null)}
                        >
                            ❌
                        </button>
                    </div>

                    <form onSubmit={handleEditSubmit}>
                        <input
                            value={editForm.description}
                            onChange={(e) =>
                                setEditForm({ ...editForm, description: e.target.value })
                            }
                            required
                        />
                        <input
                            type="number"
                            step="0.01"
                            value={editForm.amount}
                            onChange={(e) =>
                                setEditForm({ ...editForm, amount: e.target.value })
                            }
                            required
                        />
                        <input
                            type="date"
                            value={editForm.date}
                            onChange={(e) =>
                                setEditForm({ ...editForm, date: e.target.value })
                            }
                            required
                        />
                        <select
                            value={editForm.categoryId}
                            onChange={(e) =>
                                setEditForm({ ...editForm, categoryId: e.target.value })
                            }
                            required
                        >
                            <option value="">-- Select Category --</option>
                            {categories.map((c) => (
                                <option key={c.id} value={c.id}>
                                    {c.name}
                                </option>
                            ))}
                        </select>
                        <label>
                            <input
                                type="checkbox"
                                checked={editForm.isIncome}
                                onChange={(e) =>
                                    setEditForm({ ...editForm, isIncome: e.target.checked })
                                }
                            />
                            Is Income
                        </label>

                        <br />
                        <button type="submit">💾 Save</button>
                        <button
                            type="button"
                            onClick={() => setEditingTransaction(null)}
                        >
                            ✖ Cancel
                        </button>
                    </form>
                </div>
            )}
        </div>
    );
}
