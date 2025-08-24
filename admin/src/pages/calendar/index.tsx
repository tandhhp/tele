import { apiCalendarData } from "@/services/calendar";
import { Branch, BRANCH_OPTIONS } from "@/utils/constants";
import { PageContainer, ProCard } from "@ant-design/pro-components";
import { useAccess, useModel } from "@umijs/max";
import { useRequest } from "@umijs/max";
import { Calendar, CalendarProps, Select } from "antd";
import dayjs, { Dayjs } from "dayjs";
import { Fragment, useEffect, useState } from "react";
import Event from "./components/event";
import Plasma from "./components/plasma";
import { CalendarOutlined, UserAddOutlined } from "@ant-design/icons";

const CalendarPage: React.FC = () => {

    const { initialState } = useModel('@@initialState');
    const { currentUser } = initialState || {};
    const [month, setMonth] = useState<number>(dayjs().month() + 1); // month is 0-indexed in dayjs
    const [year, setYear] = useState<number>(dayjs().year());
    const [branch, setBranch] = useState<Branch>(currentUser?.branch || Branch.South);
    const { data, refresh } = useRequest(() => apiCalendarData({ month, year, branch }));
    const access = useAccess();
    const [openEvent, setOpenEvent] = useState<boolean>(false);
    const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);
    const [openPlasma, setOpenPlasma] = useState<boolean>(false);

    useEffect(() => {
        refresh();
    }, [month, year, branch]);

    const dateCellRender = (value: Dayjs) => {
        const listData = (data ?? []).filter((item: any) => item.day === value.date());
        if (listData.length === 0) {
            return <Fragment />; // No data for this date
        }
        if (listData[0].eventCount || listData[0].plasmaCount) {
            return (
                <div className="px-2">
                    {listData[0].eventCount > 0 && (
                        <div className="flex justify-between items-center bg-green-100 py-1 px-2 rounded font-semibold mb-1" onClick={() => {
                            setSelectedDate(value);
                            setOpenEvent(true);
                        }}>
                            <div><CalendarOutlined className="text-green-500" /> sự kiện</div>
                            <div className="w-5 h-5 rounded bg-green-500 text-white flex items-center justify-center text-xs">
                                {listData[0].eventCount}
                            </div>
                        </div>
                    )}
                    {listData[0].plasmaCount > 0 && (
                        <div className="flex justify-between items-center bg-orange-100 py-1 px-2 rounded font-semibold mb-1" onClick={() => {
                            setSelectedDate(value);
                            setOpenPlasma(true);
                        }}>
                            <div><UserAddOutlined className="text-orange-500" /> khách Plasma</div>
                            <div className="w-5 h-5 rounded bg-orange-500 text-white flex items-center justify-center text-xs">
                                {listData[0].plasmaCount}
                            </div>
                        </div>
                    )}
                </div>
            );
        }
        return (
            <ul className="events">
                {listData[0]?.items.map((item: any, idx: number) => (
                    <li key={idx}>
                        {item.content}
                    </li>
                ))}
            </ul>
        );
    };

    const cellRender: CalendarProps<Dayjs>['cellRender'] = (current, info) => {
        if (current.month() + 1 !== month || current.year() !== year) {
            return <Fragment />; // Only render cells for the current month and year
        }
        if (info.type === 'date') return dateCellRender(current);
        return info.originNode;
    };

    return (
        <PageContainer extra={<Select options={BRANCH_OPTIONS} defaultValue={initialState?.currentUser?.branch} onChange={(value) => {
            setBranch(value as Branch);
        }} disabled={!access.canAdmin} />}>
            <ProCard title={<div className="font-bold">Lịch làm việc</div>} headerBordered>
                <Calendar fullscreen cellRender={cellRender} onChange={(date) => {
                    setMonth(date.month() + 1); // month is 0-indexed in dayjs
                    setYear(date.year());
                }} />
            </ProCard>
            <Event open={openEvent} onClose={() => setOpenEvent(false)} selectedDate={selectedDate} />
            <Plasma open={openPlasma} onClose={() => setOpenPlasma(false)} selectedDate={selectedDate} />
        </PageContainer>
    )
}
export default CalendarPage;