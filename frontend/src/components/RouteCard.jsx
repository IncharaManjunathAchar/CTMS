function RouteCard({ route }) {
  return (
    <div className='card p-3 shadow mb-3'>
      <h4>{route.routeName}</h4>

      <p>
        {route.source} → {route.destination}
      </p>

      <h6>Stops:</h6>

      <ul>
        {route.stops?.map((stop, index) => (
          <li key={index}>{stop}</li>
        ))}
      </ul>
    </div>
  )
}

export default RouteCard