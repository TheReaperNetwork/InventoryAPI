const http = require('http');
const url = require('url');
const port = process.env.PORT || 5001;

const students = [
  { id: 1, studentId: 'S001', fullName: 'Liam Walker' },
  { id: 2, studentId: 'S002', fullName: 'Olivia Bennett' },
  { id: 3, studentId: 'S003', fullName: 'Noah Carter' },
  { id: 4, studentId: 'S004', fullName: 'Emma Mitchell' }
];

const instructors = [
  { id: 1, instructorId: 'I001', fullName: 'Ethan Marshall' },
  { id: 2, instructorId: 'I002', fullName: 'Harper Griffin' },
  { id: 3, instructorId: 'I003', fullName: 'Jack Hamilton' }
];

const products = [
  { id: 1, itemName: 'Widget', quantity: 10, minimumStockLevel: 2 },
  { id: 2, itemName: 'Gadget', quantity: 5, minimumStockLevel: 1 }
];

const server = http.createServer((req, res) => {
  const parsed = url.parse(req.url, true);
  res.setHeader('Content-Type', 'application/json');
  // CORS
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET,POST,OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type');

  if (req.method === 'OPTIONS') {
    res.statusCode = 204;
    return res.end();
  }

  if (parsed.pathname === '/api/withdrawals/students' && req.method === 'GET') {
    return res.end(JSON.stringify(students));
  }

  if (parsed.pathname === '/api/withdrawals/instructors' && req.method === 'GET') {
    return res.end(JSON.stringify(instructors));
  }

  if (parsed.pathname === '/api/products' && req.method === 'GET') {
    return res.end(JSON.stringify(products));
  }

  if (parsed.pathname === '/api/withdrawals' && req.method === 'POST') {
    let body = '';
    req.on('data', chunk => body += chunk);
    req.on('end', () => {
      try {
        const data = JSON.parse(body || '{}');
        if (!data.productId) {
          res.statusCode = 400;
          return res.end(JSON.stringify({ message: 'productId required' }));
        }
        return res.end(JSON.stringify({ message: 'mock withdrawal accepted', data }));
      } catch (e) {
        res.statusCode = 400;
        return res.end(JSON.stringify({ message: 'invalid json' }));
      }
    });
    return;
  }

  res.statusCode = 404;
  res.end(JSON.stringify({ message: 'not found' }));
});

server.listen(port, () => console.log(`Mock API listening on http://127.0.0.1:${port}`));
