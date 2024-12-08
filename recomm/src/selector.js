import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Select from 'react-select';
import Papa from 'papaparse';

const Selector = () => {
  const [options, setOptions] = useState([]);
  const [selectedOptions, setSelectedOptions] = useState([]);

  useEffect(() => {
    axios.get('municipalies.csv')//addresses
      .then(response => {
        Papa.parse(response.data, {
          header: false,
          complete: (results) => {
            const municipalities = results.data.map(row => ({ label: row[0], value: row[0] }));
            setOptions(municipalities);
          }
        });
      })
      .catch(error => {
        console.error('Error fetching the municipalities:', error);
      });
  }, []);

  const handleChange = (selected) => {
    if (selected.length <= 5) {
      setSelectedOptions(selected);
    }
  };

  return (
    <div>
      <h1>Select up to 5 Municipalities</h1>
      <Select
        options={options}
        isMulti
        value={selectedOptions}
        onChange={handleChange}
        placeholder="Select one or more"
        noOptionsMessage={() => "No options"}
      />
    </div>
  );
};

export default Selector;