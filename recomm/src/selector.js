import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Select from 'react-select';
import Papa from 'papaparse';

const Selector = () => {
  const [allOptions, setAllOptions] = useState([]);
  const [filteredOptions, setFilteredOptions] = useState([]);
  const [inputValue, setInputValue] = useState('');
  const [selectedOptions, setSelectedOptions] = useState([]);

  useEffect(() => {
    axios.get('addresses.csv')
      .then(response => {
        Papa.parse(response.data, {
          header: false,
          complete: (results) => {
            const municipalities = results.data.map(row => ({ label: row[0], value: row[0] }));
            setAllOptions(municipalities);
          }
        });
      })
      .catch(error => {
        console.error('Error fetching the municipalities:', error);
      });
  }, []);

  const handleInputChange = (input) => {
    setInputValue(input);
    if (input.length > 0) {
      const filtered = allOptions.filter(option => 
        option.label.toLowerCase().includes(input.toLowerCase())
      ).slice(0, 10);
      setFilteredOptions(filtered);
    } else {
      setFilteredOptions([]);
    }
  };

  const handleChange = (selected) => {
    setSelectedOptions(selected);
  };

  return (
    <div>
      <h1>Select up to 5 Municipalities</h1>
      <Select
        options={filteredOptions}
        onInputChange={handleInputChange}
        onChange={handleChange}
        inputValue={inputValue}
        isMulti
        value={selectedOptions}
        placeholder="Select one or more"
        noOptionsMessage={() => ""}
      />
    </div>
  );
};

export default Selector;