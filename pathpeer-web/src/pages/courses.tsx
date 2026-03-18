import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import client from '../shared/api/axiosClient'
import type { Course } from '../shared/types'

const Courses = () => {
    const navigate = useNavigate()

    const { data, isLoading, isError } = useQuery<Course[]>({
        queryKey: ['courses'],
        queryFn: () => client.get('/courses').then(r => r.data)
    })

    if (isLoading) return <div>Se încarcă cursurile...</div>
    if (isError) return <div>Eroare la încărcarea cursurilor</div>

    return (
        <div style={{ padding: '40px 60px' }}>
            <h1>Cursuri</h1>
            <button onClick={() => navigate('/')}>← Înapoi</button>

            {data?.length === 0 && <p>Nu există cursuri încă.</p>}

            <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: 20 }}>
                <thead>
                    <tr style={{ borderBottom: '2px solid #ccc', textAlign: 'left' }}>
                        <th style={{ padding: '10px 16px' }}>Titlu</th>
                        <th style={{ padding: '10px 16px' }}>Creator</th>
                        <th style={{ padding: '10px 16px' }}>Status</th>
                        <th style={{ padding: '10px 16px' }}>Preț</th>
                    </tr>
                </thead>
                <tbody>
                    {data?.map(course => (
                        <tr
                            key={course.id}
                            onClick={() => navigate(`/courses/${course.id}`)}
                            style={{
                                borderBottom: '1px solid #eee',
                                cursor: 'pointer'
                            }}
                        >
                            <td style={{ padding: '14px 16px', fontWeight: 'bold' }}>{course.title}</td>
                            <td style={{ padding: '14px 16px', color: '#888' }}>{course.creatorUsername}</td>
                            <td style={{ padding: '14px 16px', color: '#888' }}>{course.status}</td>
                            <td style={{ padding: '14px 16px' }}>{course.price} lei</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default Courses