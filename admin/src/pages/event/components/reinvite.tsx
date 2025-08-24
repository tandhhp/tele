import { apiLeadLeadHistory } from "@/services/contact";
import { ClockCircleOutlined } from "@ant-design/icons";
import { ActionType, ProTable } from "@ant-design/pro-components";
import { Modal, ModalProps } from "antd"
import dayjs from "dayjs";
import { useEffect, useRef } from "react";

type Props = ModalProps & {
    leadId?: string;
}

const ReinviteModal: React.FC<Props> = (props) => {

    const actionRef = useRef<ActionType>();

    useEffect(() => {
        if (props.open) {
            actionRef.current?.reload();
        }
    }, [props.leadId, props.open]);

    return (
        <Modal centered title="Lịch sử mời" {...props} width={1000} footer={false}>
            <ProTable
                request={(params) => apiLeadLeadHistory(params, props.leadId)}
                actionRef={actionRef}
                search={false}
                ghost
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'name'
                    },
                    {
                        title: 'SDT',
                        dataIndex: 'phoneNumber'
                    },
                    {
                        title: 'Trợ lý tiếp',
                        dataIndex: 'saleName'
                    },
                    {
                        title: 'Người gọi',
                        dataIndex: 'teleName'
                    },
                    {
                        title: 'Ngày mời',
                        dataIndex: 'eventDate',
                        render: (_, entity) => (
                            <div>
                                <div>{dayjs(entity.eventDate).format('DD-MM-YYYY')}</div>
                                <div className="text-gray-600"><ClockCircleOutlined /> {entity.eventTime}</div>
                            </div>
                        )
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'tableStatus'
                    },
                    {
                        title: 'Ghi chú',
                        dataIndex: 'note'
                    }
                ]}
            />
        </Modal>
    )
}

export default ReinviteModal;