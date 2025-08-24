import { apiCalendarPlasma } from "@/services/calendar";
import { ActionType, ProTable } from "@ant-design/pro-components";
import { useModel } from "@umijs/max";
import { Drawer, DrawerProps } from "antd";
import { Dayjs } from "dayjs";
import { useEffect, useRef } from "react";

type Props = DrawerProps & {
    selectedDate?: Dayjs | null;
}

const Plasma: React.FC<Props> = (props) => {

    const { initialState } = useModel("@@initialState");
    const actionRef = useRef<ActionType>();

    useEffect(() => {
        if (props.open && props.selectedDate) {
            actionRef.current?.reload?.();
        }
    }, [props.open, props.selectedDate]);

    return (
        <>
            <Drawer {...props} title="Plasma" width={800}>
                <ProTable
                    actionRef={actionRef}
                    request={(params) => apiCalendarPlasma({
                        ...params,
                        date: props.selectedDate ? props.selectedDate.format("YYYY-MM-DD") : undefined,
                        branch: initialState?.currentUser?.branch
                    })}
                    search={false}
                    ghost
                    columns={[
                        {
                            title: "#",
                            valueType: "indexBorder",
                            width: 30
                        },
                        {
                            title: 'Họ và tên',
                            dataIndex: 'fullName',
                            minWidth: 150,
                        },
                        {
                            title: 'Thời gian',
                            dataIndex: 'time',
                            search: false
                        },
                        {
                            title: 'Trợ lý',
                            dataIndex: 'assistant',
                            search: false
                        }
                    ]}
                />
            </Drawer>
        </>
    );
};

export default Plasma;