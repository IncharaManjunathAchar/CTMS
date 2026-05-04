function BusCard({ bus }) {
  return (
    <div className='card p-3 shadow mb-3'>
      <h4>{bus.busName}</h4>

      <h5>Bus Number: {bus.busNumber}</h5>

      <p>ETA: {bus.eta}</p>
    </div>
  )
}

export default BusCard