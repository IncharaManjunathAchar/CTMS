import { useEffect, useState } from 'react'

import Navbar from '../components/Navbar'
import BusCard from '../components/BusCard'
import API from '../services/api'

function Buses() {
  const [buses, setBuses] = useState([])

  useEffect(() => {
    fetchBuses()
  }, [])

  const fetchBuses = async () => {
    try {
      const response = await API.get('/fleet')

      setBuses(response.data)
    } catch (error) {
      console.log(error)
      alert('Failed to fetch buses')
    }
  }

  return (
    <div>
      <Navbar />

      <div className='container mt-5'>
        <h2 className='mb-4'>Available Buses</h2>

        {buses.map((bus, index) => (
          <BusCard key={index} bus={bus} />
        ))}
      </div>
    </div>
  )
}

export default Buses