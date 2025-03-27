import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getMonths, createMonth, deleteMonth } from '../services/monthApi';

export default function Dashboard() {
    const [months, setMonths] = useState([]);
    const [showForm, setShowForm] = useState(false);
    const [yearInput, setYearInput] = useState('');
    const [monthInput, setMonthInput] = useState('');
    const navigate = useNavigate();

    const fetchMonths = async () => {
        try {
            const token = localStorage.getItem('token'); 
            const data = await getMonths(token);         
            setMonths(data);
        } catch (err) {
            console.error('Error fetching months', err);
        }
    };

    useEffect(() => {
        fetchMonths();
    }, []);

    const handleAddMonth = async (e) => {
        e.preventDefault();
        try {
            const token = localStorage.getItem('token');
            await createMonth(token, parseInt(yearInput), parseInt(monthInput));
            fetchMonths();
            
            setYearInput('');
            setMonthInput('');
            setShowForm(false);
        } catch (err) {
            console.error('Error creating month', err);
        }
    };

    const handleDeleteMonth = async (id) => {
        if (!window.confirm('Delete this month?')) return;
        try {
            const token = localStorage.getItem('token');
            await deleteMonth(token, id);
            fetchMonths();
        } catch (err) {
            console.error('Error deleting month', err);
        }
    };
    
    const monthNames = [
        '', 'January', 'February', 'March',
        'April', 'May', 'June', 'July',
        'August', 'September', 'October',
        'November', 'December'
    ];

    return (
        <div id="dashboard">
            <h2>Your Available Months</h2>
            
            {months.length === 0 && (
                <p>No months yet.</p>
            )}
            
            <button onClick={() => setShowForm(!showForm)}>+ Add Month</button>

            {showForm && (
                <form onSubmit={handleAddMonth}>
                    {/* Wybór roku */}
                    <select
                        value={yearInput}
                        onChange={(e) => setYearInput(e.target.value)}
                        required
                    >
                        <option value="">-- Choose Year --</option>
                        {[...Array(11)].map((_, i) => {
                            const y = 2020 + i; // od 2020 do 2030
                            return (
                                <option key={y} value={y}>
                                    {y}
                                </option>
                            );
                        })}
                    </select>

                    {/* Wybór miesiąca */}
                    <select
                        value={monthInput}
                        onChange={(e) => setMonthInput(e.target.value)}
                        required
                    >
                        <option value="">-- Choose Month --</option>
                        <option value="1">January</option>
                        <option value="2">February</option>
                        <option value="3">March</option>
                        <option value="4">April</option>
                        <option value="5">May</option>
                        <option value="6">June</option>
                        <option value="7">July</option>
                        <option value="8">August</option>
                        <option value="9">September</option>
                        <option value="10">October</option>
                        <option value="11">November</option>
                        <option value="12">December</option>
                    </select>

                    <button type="submit">Create</button>
                </form>
            )}
            
            {months.map((m) => (
                <div key={m.id} style={{ marginTop: '10px' }}>
                    <button onClick={() => navigate(`/month/${m.year}/${m.month}`)}>
                        {monthNames[m.month]} {m.year}
                    </button>
                    <button onClick={() => handleDeleteMonth(m.id)}>Delete</button>
                </div>
            ))}
        </div>
    );
}
