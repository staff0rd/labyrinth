import React from 'react';
import SvgIcon, { SvgIconProps } from '@material-ui/core/SvgIcon';

// <a target="_blank" href="https://icons8.com/icons/set/microsoft-team-2019">Microsoft Teams 2019 icon</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>
export default (props: SvgIconProps) => {
    return (
        <SvgIcon {...props}>
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 48 48">
                <path d="M42,20H32v10c0,3.314,2.686,6,6,6s6-2.686,6-6v-8C44,20.895,43.105,20,42,20z"/>
                <path d="M15,20v12.657c0,5.326,4.019,9.977,9.334,10.321C30.159,43.356,35,38.743,35,33V22 c0-1.105-0.895-2-2-2H15z"/>
                <path d="M25 5A6 6 0 1 0 25 17A6 6 0 1 0 25 5Z"/>
                <path d="M38 8A4 4 0 1 0 38 16A4 4 0 1 0 38 8Z"/>
                <path d="M22.319,34H5.681C4.753,34,4,33.247,4,32.319V15.681C4,14.753,4.753,14,5.681,14h16.638 C23.247,14,24,14.753,24,15.681v16.638C24,33.247,23.247,34,22.319,34z"/>
                <path fill="#212121" d="M18.068 18.999L9.932 18.999 9.932 20.719 12.979 20.719 12.979 28.999 15.021 28.999 15.021 20.719 18.068 20.719z"/>
            </svg>
        </SvgIcon>
    );
}
