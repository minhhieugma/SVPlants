import React from 'react';
import moment from 'moment';

export default function DateTimeDisplay(props) {
  return (
    <>{props.value ? moment(props.value).format("YYYY-MM-DD HH:mm:ss") : '-'}</>
  )
}
